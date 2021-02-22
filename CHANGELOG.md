# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [0.1.1] - 2021-02-21

Sets up CI for pushing docker image to ACR.

### Added
- Azure Pipeline for pushing docker image to ACR when new git tags are pushed
to GitHub.

## [0.1.0] - 2021-02-18

Initial release.

### Added

- `GET /api/accounts`
- `GET /api/accounts/<id>`
- `GET /api/transactions`
- `GET /api/transactions/<id>`
- `POST /api/accounts`, which expects form data in format `application/x-www-form-urlencoded`.

  Required fields:

  - `name`, name of the new account
  - `increaseBalanceOn`, `0` to increase balance on debit or `1` to increase balance on credit

- `POST /api/transactions`, which expects form data in format `application/x-www-form-urlencoded`.

  Required fields:

  - `creditAccount`, name of credit account
  - `debitAccount`, name of debit account
  - `amount`, amount of currency
  - `currency`, currency, for example `SEK`
  - `date`, ISO string of date of transaction

  Optional fields:

  - `description`, description of transaction
