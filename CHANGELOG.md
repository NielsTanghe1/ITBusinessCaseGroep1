# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project (tries) to adhere to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.6.1] - 2026-01-15

### Fixed

Updated namespaces and references in HomeController.cs, ErrorViewModel.cs, and _ViewImports.cs from ITBusinessCase to Web to ensure proper project linkage and consistency.

## [1.6.0] - 2026-01-15

### Added

Introduced LocalDbContext in `/Models/Data/` to define database structure, configuration, and entity relationships.

Implemented in LocalDbContext:
- DbSet properties for all entities (excluding base entities).
- OnModelCreating() for key naming, identity property configuration, and enum conversions.
- OnConfiguring() for SQL Server connection setup via configuration.
- Seeder() method to populate test data using entity SeedingData() methods.

Added initial create migration (InitialCreate) and supporting files in /Models/Migrations/.

### Changed

Updated /Web/appsettings.json to restore ConnectionStrings with a LocalDbContextConnection entry.

## [1.5.0] - 2026-01-15

### Changed

Renamed Customer entity and related references to CoffeeUser for consistent domain terminology.

Expanded CoffeeUser.SeedingData() with four new sample users and marked all seed user emails as confirmed.

Updated entity relationships in Address, Order, OrderItem, and PaymentDetail to explicitly define DeleteBehavior.

Added data annotations to price-related properties for validation and precision:
- Coffee.Price: restricted range between 0.00 and 50.00.
- OrderItem.PriceAtPurchase: restricted range between 0.00 and 150000.00.

Updated PaymentDetail.Id display name from PaymentInfoId to PaymentDetailsId.

## [1.4.1] - 2026-01-14

### Fixed

Corrected `Address.cs` to reference AddressType.Personal instead of the invalid AddressType.Home value.

## [1.4.0] - 2026-01-13

Updated all entity models in `/Models/Entities/` to align with new domain insights and support predefined seeding data.

### Added

Added SeedingData methods to multiple entities (Address, Coffee, Customer, Order, OrderItem, PaymentDetail) for generating sample data.

Replaced Product and ProductType with new Coffee and CoffeeType models for domain accuracy.

Added CoffeeName enum defining supported coffee varieties (Unknown, Arabica, Robusta, Liberica, Excelsa).

### Changed

Adjusted enumeration values:
- AddressType: added Business.
- CoffeeType: simplified to relevant values (Unknown, Roasted, Grounded, Raw).
- StatusType: removed Shipped; added Confirmed and Canceled.

Updated Order entity to make Status optional with a default value of Pending.

Updated OrderItem to reference the new Coffee type	and name instead of Product.

Changed PaymentDetail.LastFour type from string to int for stronger typing and data consistency.

## [1.3.0] - 2026-01-13

### Added

Introduced initial entity models defined by the ERD schema:
- Added base classes (BaseEntity, BaseIdentityEntity) to centralize common properties such as creation and deletion timestamps.
- Added main entities: Address, Customer, Order, OrderItem, PaymentDetail, and Product.
- Added supporting enum-like types (AddressType, ProductType, StatusType) to enforce model consistency and type safety.

## [1.2.0] - 2026-01-12

### Added

Add new Models project with initial structure and configuration.
- Add relevant files:
  - `Models.csproj` and `Models.sln`.

Add folders to Models:
- `Entities/DTO/`
- `Extensions/Mappings/`
- `Migrations/`
- `Data/`
- `Resources/`

## [1.1.0] - 2026-01-09

### Changed

- Reorganized web application structure by moving all web-related files into a `/Web` directory for improved clarity and maintainability.
- Updated `ITBusinessCase.csproj` project name and references to the relocated `Web.csproj` project files.
- Updated GitHub Actions workflow (`.github/workflows/dotnet.yml`) to reference `Web/Web.sln` for .NET commands.
- Corrected [README](README.md) image reference from `image-1.png` to `route_visual_1.jpg`.

## [1.0.2] - 2026-01-08

### Added

- Added `/Resources/Images/` folder and `route_visual_1.png` to illustrate the data flow between the app, RabbitMQ, and Salesforce.

### Changed

- Expanded README with shields.io badges, a richer description, and new "Getting Started", "Sources", and "Contributors" sections.

## [1.0.1] - 2026-01-06

### Added

- Added CHANGELOG.md.
- Added NuGet package reference for RabbitMQ.Client in `ITBusinessCase.csproj`.

### Changed

- Added a **VSSpell** ignored words section to [.editorconfig](.editorconfig).

## [1.0.0] - 2026-01-06

- Initial commit: ASP.NET Core web app.
  - Added default ASP.NET Core web app files.
  - Added [.editorconfig](.editorconfig).
  - Added [.gitignore](.gitignore).

## [0.0.4] - 2026-01-06

- Added GitHub Actions workflow for .NET tests.

## [0.0.3] - 2026-01-05

- Added GitHub Actions workflow for Laravel tests.

## [0.0.2] - 2026-01-05

- Added basic GitHub Actions workflow.

## [0.0.1] - 2026-01-05

- Initial commit: creates repository.

[1.6.1]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.6.1
[1.6.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.6.0
[1.5.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.5.0
[1.4.1]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.4.1
[1.4.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.4.0
[1.3.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.3.0
[1.2.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.2.0
[1.1.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.1.0
[1.0.2]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.0.2
[1.0.1]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.0.1
[1.0.0]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v1.0.0
[0.0.4]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v0.0.4
[0.0.3]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v0.0.3
[0.0.2]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v0.0.2
[0.0.1]: https://github.com/NielsTanghe1/ITBusinessCaseGroep1/releases/tag/v0.0.1