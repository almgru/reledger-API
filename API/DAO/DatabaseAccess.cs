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
            this.InsertAccountAndLinkToDescendants(debitAccount);
            this.InsertAccountAndLinkToDescendants(creditAccount);

            Int64 id = this.InsertTransaction(amount, currency, date, description);

            foreach (string tag in tags)
            {
                this.InsertTag(tag);
                this.LinkTagToTransaction(tag, id);
            }

            this.LinkTransactionToDebitAccount(id, debitAccount);
            this.LinkTransactionToCreditAccount(id, creditAccount);

            foreach ((string, byte[]) tuple in attachments)
            {
                this.InsertAttachment(tuple.Item1, id, tuple.Item2);
            }
        }

        private Int64 InsertTransaction(decimal amount, string currency, DateTime date, string description)
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
        private void InsertAccountAndLinkToDescendants(string accountString)
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

                    this.ExecuteCommand(connection, "INSERT INTO Accounts(name) VALUES(:name);",
                                        (":name", accountName));

                    this.LinkAncestorAccountToDescendantAccounts(connection, accountName, descendants);

                    // Remove the ancestor from the account string
                    accountString = accountString.Remove(0, indexOfDot).Trim();
                }

                transaction.Commit();
            }
        }

        private void InsertTag(string name)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection, "INSERT INTO Tags(name) VALUES(:name);", (":name", name));

                transaction.Commit();
            }
        }

        private void InsertAttachment(string name, Int64 transactionId, byte[] data)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO Attachments(name, transactionId, data)
                        VALUES(:name, :transactionId, :data);
                    ",
                    (":name", name), (":transactionId", transactionId), (":data", data)
                );

                transaction.Commit();
            }
        }

        private void LinkAncestorAccountToDescendantAccounts(SqliteConnection connection, string ancestorName,
                                                             string descendantString)
        {
            foreach (string descendant in descendantString.Split('.'))
            {
                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO AncestorTo(ancestorName, descendantName)
                        VALUES(:ancestorName, :descendantName);
                    ",
                    (":ancestorName", ancestorName), (":descendantName", descendant)
                );
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

        private void LinkTagToTransaction(string tagName, Int64 transactionId)
        {
            using (var connection = this.GetConnection())
            {
                var transaction = connection.BeginTransaction();

                this.ExecuteCommand(connection,
                    @"
                        INSERT INTO Categorizes(tagName, transactionId)
                        VALUES(:tagName, :transactionId);
                    ",
                    (":tagName", tagName), (":transactionId", transactionId)
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