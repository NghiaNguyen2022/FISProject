DROP TABLE "tb_Payoo_PaymentINT"

CREATE TABLE "tb_Payoo_PaymentINT"
(
	"PaymentMethod"	INT,
	"PaymentMethodName"	NVARCHAR(50),
	"PurDate" NVARCHAR(20),
	"MerchantUserName" NVARCHAR(50),
	"ShopId" NUMERIC,
	"MasterShopID" NUMERIC,
	"OrderNo" NVARCHAR(50),
	"OrderCash" DECIMAL(19,2),
	"BankName" NVARCHAR(50),
	"CardNumber" NVARCHAR(50),
	"CardIssuanceType" INT,
	"PaymentStatus" INT,
	"MDD1" NVARCHAR(50),
	"MDD2" NVARCHAR(50),
	"PaymentSource" NVARCHAR(50),
	"VoucherTotalAmount" DECIMAL(19,2),
	"VoucherID" NVARCHAR(50),
	"IntDate" DATE,
	"IntTime" NVARCHAR(6), -- 'HHmmss',
	"PaymentMethodSub"	INT
)
ALTER TABLE "tb_Payoo_PaymentINT"
ADD ("PaymentMethodSub"	INT)

CREATE TABLE "tb_Payoo_PaymentOrder"
(
	"OrderParentNo" NVARCHAR(50),
	"Seller" NVARCHAR(50),
	"Shop" NUMERIC,
	"Description" NVARCHAR(250),
	"Total" DECIMAL(19,2),
	"OrderNo" NVARCHAR(50)
)

ALTER PROCEDURE "usp_Payoo_CheckOrderExists"
(
	IN v_OrderNo NVARCHAR(50),
	IN v_SAPNo NVARCHAR(50)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	IF EXISTS (SELECT 1
				 FROM OINV 
				WHERE CAST("DocNum" AS NVARCHAR(50)) = :v_SAPNo)
	THEN
		IF EXISTS (SELECT 1
					 FROM "tb_Payoo_PaymentINT" 
				    WHERE "OrderNo" = :v_OrderNo)
		THEN
			SELECT 'Existed' AS "Existed" FROM DUMMY;
		ELSE	
			SELECT 'No' AS "Existed" FROM DUMMY;
		END IF;
	ELSE
		SELECT '-1' AS "Existed" FROM DUMMY;
	END IF;
	
END


CREATE PROCEDURE "usp_Bank_PayooInvoiceNoPay"
(
	IN v_CardCode NVARCHAR(40),
	IN v_TadIdNumber NVARCHAR(40)
)
LANGUAGE SQLSCRIPT
AS
BEGIN	
	SELECT T0."DocNum" AS "DocEntry",
	 	   CASE WHEN T0."NumAtCard" LIKE '%.%' THEN T0."NumAtCard"
	 	   		ELSE ''
	 	   	END AS "VatInvoiceNumber",
	 	   t0."DocDate",
	 	   T3."InsTotal",
	 	   T3."PaidToDate", 
	 	   T3."InsTotal" - T3."PaidToDate" AS "Remain"
	  FROM OINV T0 
	  JOIN OCRD T1 ON T0."CardCode" = t1."CardCode"
	  JOIN INV6 T3 ON T0."DocEntry" = T3."DocEntry"
	 WHERE T1."CardCode" = :v_CardCode
	   AND T1."LicTradNum" = :v_TadIdNumber
	   AND T0."PeyMethod" <> 'COD' 
	   AND T3."InsTotal" - T3."PaidToDate"  > 0 ;
END