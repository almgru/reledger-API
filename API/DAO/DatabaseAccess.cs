using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;

namespace API.DAO
{
    public class DatabaseAccess
    {
        private readonly string connectionString;

        public DatabaseAccess(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public void AddTransaction(string debitAccount, decimal amount, string currency, string creditAccount,
                                   DateTime date, string description, List<string> tags,
                                   List<(string, byte[])> attachments)
        {
            // 1. Insert debit account into database in case its missing
            this.AddAccount(debitAccount);
            // 2. Insert credit account into database in case its missing
            this.AddAccount(creditAccount);
            // 3. Insert the transaction into the database
            Int64 id = this.AddTransaction(amount, currency, date, description);
            // 4. Add all tags into the database in case any are missing
            foreach (string tag in tags)
            {
                this.AddTag(tag, id);
            }
            // 5. Link the transaction to the debited account by inserting its ID and the debit account name into the
            //    'Debits' table.
            this.LinkTransactionToDebitAccount(id, debitAccount);
            // 6. Link the transaction to the credited account by inserting its ID and the credit account name into the
            //    'Credits' table.
            this.LinkTransactionToCreditAccount(id, creditAccount);
            // 7. For each attachment, link it to the transaction by inserting the transaction ID, the attachment name,
            //    and the binary blob into the 'Attachments' table.
        }

        private Int64 AddTransaction(decimal amount, string currency, DateTime date, string description)
        {
            Int64 lastRowId;

            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                lastRowId = (Int64)this.ExecuteScalarCommand(connection,
                    @"
                        INSERT INTO Transactions(date, amount, currency, description)
                        VALUES(:date, :amount, :currency, :description);
                    ",
                    (":date", date), (":amount", amount), (":currency", currency), (":description", description)
                );

                transaction.Commit();
            }

            return lastRowId;
        }

        /*
         * Insert all accounts specified in the accountString and registers any ancestors/descendants by
         * inserting into the AncestorTo table.
         *
         * The insertion happens in the order most ancestral to most descendant.
         *
         * Example:
         *
         * AddAccount("A.B.C") will first insert account 'A' to the 'Accounts' table and add 'B' and 'C' as descendants
         * of 'A' by adding them to the 'AncestorOf' table. Next, 'B' will be inserted into 'Accounts' and 'C' will be
         * added as its decendant. Finally, 'C' will be inserted into 'Accounts'. Since 'C' has no descendants, the
         * 'AncestorTo' table will not be further modified.
         */
        private void AddAccount(string accountString)
        {
            using (var connection = GetConnection())
            {
                SqliteTransaction transaction = connection.BeginTransaction();

                while (accountString != "") // Repeat until no more account names in the account string
                {
                    // Extract first account name as ancestor and the rest of the string as its descendants
                    int indexOfDot = accountString.IndexOf('.');
                    string accountName = accountString.Substring(0, indexOfDot);
                    string descendants = accountString.Substring(indexOfDot);

                    // Add the ancestor
                    this.ExecuteCommand(connection, "INSERT INTO Accounts(name) VALUES(:name);",
                                        (":name", accountName));

                    foreach (string descendant in descendants.Split('.')) // for each descendant
                    {
                        // Link the ancestor to the descendant
                        this.ExecuteCommand(connection,
                            @"
                                INSERT INTO AncestorTo(ancestorName, descendantName)
                                VALUES(:ancestorName, :descendantName);
                            ",
                            (":ancestorName", accountName), (":descendantName", descendant)
                        );
                    }

                    // Remove the ancestor from the account string
                    accountString = accountString.Remove(0, indexOfDot).Trim();
                }

                transaction.Commit();
            }
        }

        private void AddTag(string name, Int64 transactionId)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection, "INSERT INTO Tags(name) VALUES(:name);", (":name", name));
                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO Categorizes(tagName, transactionId)
                        VALUES(:tagName, :transactionId);
                    ",
                    (":tagName", name), (":transactionId", transactionId)
                );

                transaction.Commit();
            }
        }

        private void LinkTransactionToDebitAccount(Int64 transactionId, string accountName)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO Debits(transactionId, accountName)
                        VALUES(:transactionId, :accountName);
                    ",
                    (":transactionId", transactionId), (":accountName", accountName)
                );

                transaction.Commit();
            }
        }

        private void LinkTransactionToCreditAccount(Int64 transactionId, string accountName)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO Credits(transactionId, accountName)
                        VALUES(:transactionId, :accountName);
                    ",
                    (":transactionId", transactionId), (":accountName", accountName)
                );

                transaction.Commit();
            }
        }

        public void CreateSchema()
        {
            using (var connection = GetConnection())
            {
                SqliteTransaction transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Transactions(
                        id INTEGER PRIMARY KEY NOT NULL,
                        date TEXT NOT NULL,
                        amount TEXT NOT NULL,
                        currency TEXT NOT NULL,
                        description TEXT
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Attachments(
                        name TEXT NOT NULL,
                        transactionId INTEGER NOT NULL,
                        data BLOB NOT NULL,
                        
                        PRIMARY KEY(name, transactionId),

                        FOREIGN KEY (transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Tags(
                        name TEXT PRIMARY KEY NOT NULL
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Categorizes(
                        tagName TEXT NOT NULL,
                        transactionId INTEGER NOT NULL,

                        PRIMARY KEY(tagName, transactionId),

                        FOREIGN KEY(tagName)
                        REFERENCES Tags(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Debits(
                        transactionId INTEGER NOT NULL,
                        accountName TEXT NOT NULL,

                        PRIMARY KEY(transactionId, accountName),

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(accountName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Credits(
                        transactionId INTEGER NOT NULL,
                        accountName TEXT NOT NULL,

                        PRIMARY KEY(transactionId, accountName),

                        FOREIGN KEY(transactionId)
                        REFERENCES Transactions(id)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(accountName)
                        REFERENCES Account(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE
                    )
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE Accounts(
                        name TEXT PRIMARY KEY NOT NULL
                    );
                ");

                this.ExecuteCommand(connection, @"
                    CREATE TABLE AncestorTo(
                        ancestorName TEXT NOT NULL,
                        descendantName TEXT NOT NULL,

                        PRIMARY KEY(ancestorName, descendantName),

                        FOREIGN KEY(ancestorName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE,

                        FOREIGN KEY(descendantName)
                        REFERENCES Accounts(name)
                        ON UPDATE CASCADE
                        ON DELETE CASCADE);
                ");

                transaction.Commit();
            }
        }

        private SqliteConnection GetConnection()
        {
            SqliteConnection connection = new SqliteConnection(this.connectionString);
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = "PRAGMA foreign_keys = ON;";

            connection.Open();
            command.ExecuteNonQuery();

            return connection;
        }

        private object ExecuteScalarCommand(SqliteConnection connection, string commandString,
                                            params (string, object)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandString;
            return command.ExecuteScalar();
        }

        private void ExecuteCommand(SqliteConnection connection, string commandString)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = commandString;
            command.ExecuteNonQuery();
        }

        private void ExecuteCommand(SqliteConnection connection, string commandString,
                                    params (string, object)[] parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandString;

            foreach ((string, Object) tuple in parameters)
            {
                command.Parameters.AddWithValue(tuple.Item1, tuple.Item2);
            }

            command.ExecuteNonQuery();
        }
    }
}