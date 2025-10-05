using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ec_project_api.Migrations
{
    /// <inheritdoc />
    public partial class initialcreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Resources",
                columns: table => new
                {
                    resource_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resources", x => x.resource_id);
                });

            migrationBuilder.CreateTable(
                name: "Statuses",
                columns: table => new
                {
                    status_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    entity_type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statuses", x => x.status_id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    category_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    size_detail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    parent_id = table.Column<short>(type: "smallint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.category_id);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_parent_id",
                        column: x => x.parent_id,
                        principalTable: "Categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Categories_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Colors",
                columns: table => new
                {
                    color_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    display_name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    hex_code = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Colors", x => x.color_id);
                    table.ForeignKey(
                        name: "FK_Colors_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    discount_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    discount_type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    discount_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    min_order_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    max_discount_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    usage_limit = table.Column<int>(type: "int", nullable: true),
                    used_count = table.Column<int>(type: "int", nullable: false),
                    start_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.discount_id);
                    table.ForeignKey(
                        name: "FK_Discounts_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Materials",
                columns: table => new
                {
                    material_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materials", x => x.material_id);
                    table.ForeignKey(
                        name: "FK_Materials_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    payment_method_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    method_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.payment_method_id);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    permission_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    permission_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    resource_id = table.Column<short>(type: "smallint", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.permission_id);
                    table.ForeignKey(
                        name: "FK_Permissions_Resources_resource_id",
                        column: x => x.resource_id,
                        principalTable: "Resources",
                        principalColumn: "resource_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "status_id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    role_id = table.Column<short>(type: "smallint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.role_id);
                    table.ForeignKey(
                        name: "FK_Roles_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ships",
                columns: table => new
                {
                    ship_id = table.Column<byte>(type: "tinyint", nullable: false),
                    corp_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    base_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    estimated_days = table.Column<byte>(type: "tinyint", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ships", x => x.ship_id);
                    table.ForeignKey(
                        name: "FK_Ships_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sizes",
                columns: table => new
                {
                    size_id = table.Column<byte>(type: "tinyint", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sizes", x => x.size_id);
                    table.ForeignKey(
                        name: "FK_Sizes_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    contact_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Suppliers_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    is_verified = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    status_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
                    table.ForeignKey(
                        name: "FK_Users_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    product_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    material_id = table.Column<short>(type: "smallint", nullable: false),
                    category_id = table.Column<short>(type: "smallint", nullable: false),
                    base_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    discount_percentage = table.Column<decimal>(type: "decimal(5,2)", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.product_id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_category_id",
                        column: x => x.category_id,
                        principalTable: "Categories",
                        principalColumn: "category_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Materials_material_id",
                        column: x => x.material_id,
                        principalTable: "Materials",
                        principalColumn: "material_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentDestinations",
                columns: table => new
                {
                    destination_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    payment_method_id = table.Column<int>(type: "int", nullable: true),
                    identifier = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    bank_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    account_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentDestinations", x => x.destination_id);
                    table.ForeignKey(
                        name: "FK_PaymentDestinations_PaymentMethods_payment_method_id",
                        column: x => x.payment_method_id,
                        principalTable: "PaymentMethods",
                        principalColumn: "payment_method_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_PaymentDestinations_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    role_id = table.Column<short>(type: "smallint", nullable: false),
                    permission_id = table.Column<short>(type: "smallint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.role_id, x.permission_id });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_permission_id",
                        column: x => x.permission_id,
                        principalTable: "Permissions",
                        principalColumn: "permission_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    purchase_order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    supplier_id = table.Column<int>(type: "int", nullable: false),
                    order_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.purchase_order_id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_supplier_id",
                        column: x => x.supplier_id,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    address_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    recipient_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    phone = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    street_address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    city = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ward = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    district = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    is_default = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    StatusId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.address_id);
                    table.ForeignKey(
                        name: "FK_Addresses_Statuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Statuses",
                        principalColumn: "status_id");
                    table.ForeignKey(
                        name: "FK_Addresses_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    cart_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.cart_id);
                    table.ForeignKey(
                        name: "FK_Carts_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoleDetails",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    role_id = table.Column<short>(type: "smallint", nullable: false),
                    assigned_by = table.Column<int>(type: "int", nullable: true),
                    assigned_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoleDetails", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_UserRoleDetails_Roles_role_id",
                        column: x => x.role_id,
                        principalTable: "Roles",
                        principalColumn: "role_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoleDetails_Users_assigned_by",
                        column: x => x.assigned_by,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_UserRoleDetails_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    product_image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_primary = table.Column<bool>(type: "bit", nullable: false),
                    display_order = table.Column<byte>(type: "tinyint", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.product_image_id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    product_variant_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<int>(type: "int", nullable: false),
                    color_id = table.Column<short>(type: "smallint", nullable: false),
                    size_id = table.Column<byte>(type: "tinyint", nullable: false),
                    sku = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    stock_quantity = table.Column<int>(type: "int", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.product_variant_id);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Colors_color_id",
                        column: x => x.color_id,
                        principalTable: "Colors",
                        principalColumn: "color_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_product_id",
                        column: x => x.product_id,
                        principalTable: "Products",
                        principalColumn: "product_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Sizes_size_id",
                        column: x => x.size_id,
                        principalTable: "Sizes",
                        principalColumn: "size_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    payment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    destination_id = table.Column<int>(type: "int", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    transaction_id = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    paid_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.payment_id);
                    table.ForeignKey(
                        name: "FK_Payments_PaymentDestinations_destination_id",
                        column: x => x.destination_id,
                        principalTable: "PaymentDestinations",
                        principalColumn: "destination_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Payments_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    cart_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    cart_id = table.Column<int>(type: "int", nullable: false),
                    product_variant_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => x.cart_item_id);
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_cart_id",
                        column: x => x.cart_id,
                        principalTable: "Carts",
                        principalColumn: "cart_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "ProductVariants",
                        principalColumn: "product_variant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    purchase_order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    purchase_order_id = table.Column<int>(type: "int", nullable: false),
                    product_variant_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    unit_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.purchase_order_item_id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_ProductVariants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "ProductVariants",
                        principalColumn: "product_variant_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_purchase_order_id",
                        column: x => x.purchase_order_id,
                        principalTable: "PurchaseOrders",
                        principalColumn: "purchase_order_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    order_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    address_info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    discount_id = table.Column<int>(type: "int", nullable: true),
                    total_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    is_free_ship = table.Column<bool>(type: "bit", nullable: false),
                    shipping_fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    shipped_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    delivery_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    ship_id = table.Column<byte>(type: "tinyint", nullable: true),
                    payment_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.order_id);
                    table.ForeignKey(
                        name: "FK_Orders_Discounts_discount_id",
                        column: x => x.discount_id,
                        principalTable: "Discounts",
                        principalColumn: "discount_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Payments_payment_id",
                        column: x => x.payment_id,
                        principalTable: "Payments",
                        principalColumn: "payment_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Ships_ship_id",
                        column: x => x.ship_id,
                        principalTable: "Ships",
                        principalColumn: "ship_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Orders_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    order_item_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_id = table.Column<int>(type: "int", nullable: false),
                    product_variant_id = table.Column<int>(type: "int", nullable: false),
                    quantity = table.Column<short>(type: "smallint", nullable: false),
                    price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    sub_total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => x.order_item_id);
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_order_id",
                        column: x => x.order_id,
                        principalTable: "Orders",
                        principalColumn: "order_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductVariants_product_variant_id",
                        column: x => x.product_variant_id,
                        principalTable: "ProductVariants",
                        principalColumn: "product_variant_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductReturns",
                columns: table => new
                {
                    return_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_item_id = table.Column<int>(type: "int", nullable: false),
                    return_type = table.Column<int>(type: "int", nullable: false),
                    return_reason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    return_amount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    return_product_variant_id = table.Column<int>(type: "int", nullable: true),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReturns", x => x.return_id);
                    table.ForeignKey(
                        name: "FK_ProductReturns_OrderItems_order_item_id",
                        column: x => x.order_item_id,
                        principalTable: "OrderItems",
                        principalColumn: "order_item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReturns_ProductVariants_return_product_variant_id",
                        column: x => x.return_product_variant_id,
                        principalTable: "ProductVariants",
                        principalColumn: "product_variant_id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProductReturns_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    review_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    order_item_id = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<byte>(type: "tinyint", nullable: false),
                    comment = table.Column<string>(type: "text", nullable: true),
                    is_edited = table.Column<bool>(type: "bit", nullable: false),
                    status_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.review_id);
                    table.ForeignKey(
                        name: "FK_Reviews_OrderItems_order_item_id",
                        column: x => x.order_item_id,
                        principalTable: "OrderItems",
                        principalColumn: "order_item_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Statuses_status_id",
                        column: x => x.status_id,
                        principalTable: "Statuses",
                        principalColumn: "status_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ReviewImages",
                columns: table => new
                {
                    review_image_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    review_id = table.Column<int>(type: "int", nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    alt_text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewImages", x => x.review_image_id);
                    table.ForeignKey(
                        name: "FK_ReviewImages_Reviews_review_id",
                        column: x => x.review_id,
                        principalTable: "Reviews",
                        principalColumn: "review_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Address_User_Default",
                table: "Addresses",
                columns: new[] { "user_id", "is_default" });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_StatusId",
                table: "Addresses",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_product_variant_id",
                table: "CartItems",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "UK_CartItem_Cart_ProductVariant",
                table: "CartItems",
                columns: new[] { "cart_id", "product_variant_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_user_id",
                table: "Carts",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_parent_id",
                table: "Categories",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_status_id",
                table: "Categories",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "UK_Category_Name",
                table: "Categories",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UK_Category_Slug",
                table: "Categories",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_name",
                table: "Colors",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Colors_status_id",
                table: "Colors",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_code",
                table: "Discounts",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_status_id",
                table: "Discounts",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Materials_name",
                table: "Materials",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materials_status_id",
                table: "Materials",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_order_id",
                table: "OrderItems",
                column: "order_id");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_product_variant_id",
                table: "OrderItems",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_discount_id",
                table: "Orders",
                column: "discount_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_payment_id",
                table: "Orders",
                column: "payment_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ship_id",
                table: "Orders",
                column: "ship_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_status_id",
                table: "Orders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_user_id",
                table: "Orders",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDestinations_payment_method_id",
                table: "PaymentDestinations",
                column: "payment_method_id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentDestinations_status_id",
                table: "PaymentDestinations",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_method_name",
                table: "PaymentMethods",
                column: "method_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_status_id",
                table: "PaymentMethods",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_destination_id",
                table: "Payments",
                column: "destination_id");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_status_id",
                table: "Payments",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_permission_name",
                table: "Permissions",
                column: "permission_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_resource_id",
                table: "Permissions",
                column: "resource_id");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_StatusId",
                table: "Permissions",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_product_id",
                table: "ProductImages",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReturns_order_item_id",
                table: "ProductReturns",
                column: "order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReturns_return_product_variant_id",
                table: "ProductReturns",
                column: "return_product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReturns_status_id",
                table: "ProductReturns",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_category_id",
                table: "Products",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_material_id",
                table: "Products",
                column: "material_id");

            migrationBuilder.CreateIndex(
                name: "IX_Products_name",
                table: "Products",
                column: "name");

            migrationBuilder.CreateIndex(
                name: "IX_Products_slug",
                table: "Products",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_status_id",
                table: "Products",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_color_id",
                table: "ProductVariants",
                column: "color_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_product_id",
                table: "ProductVariants",
                column: "product_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_size_id",
                table: "ProductVariants",
                column: "size_id");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_sku",
                table: "ProductVariants",
                column: "sku",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_status_id",
                table: "ProductVariants",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_product_variant_id",
                table: "PurchaseOrderItems",
                column: "product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_purchase_order_id",
                table: "PurchaseOrderItems",
                column: "purchase_order_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_status_id",
                table: "PurchaseOrders",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_supplier_id",
                table: "PurchaseOrders",
                column: "supplier_id");

            migrationBuilder.CreateIndex(
                name: "IX_Resources_name",
                table: "Resources",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_review_id",
                table: "ReviewImages",
                column: "review_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_order_item_id",
                table: "Reviews",
                column: "order_item_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_status_id",
                table: "Reviews",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_permission_id",
                table: "RolePermissions",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_name",
                table: "Roles",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_status_id",
                table: "Roles",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ships_corp_name",
                table: "Ships",
                column: "corp_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ships_status_id",
                table: "Ships",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_name",
                table: "Sizes",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sizes_status_id",
                table: "Sizes",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Statuses_name_entity_type",
                table: "Statuses",
                columns: new[] { "name", "entity_type" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_email",
                table: "Suppliers",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_name",
                table: "Suppliers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_phone",
                table: "Suppliers",
                column: "phone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_status_id",
                table: "Suppliers",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleDetails_assigned_by",
                table: "UserRoleDetails",
                column: "assigned_by");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoleDetails_role_id",
                table: "UserRoleDetails",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_email",
                table: "Users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_phone",
                table: "Users",
                column: "phone",
                unique: true,
                filter: "[phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_status_id",
                table: "Users",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_username",
                table: "Users",
                column: "username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "ProductReturns");

            migrationBuilder.DropTable(
                name: "PurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "ReviewImages");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoleDetails");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "Resources");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Ships");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Colors");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Sizes");

            migrationBuilder.DropTable(
                name: "PaymentDestinations");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Materials");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "Statuses");
        }
    }
}
