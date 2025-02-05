ALTER PROCEDURE "usp_Bank_PayooInvoiceNoPay"
(
	IN v_CardCode NVARCHAR(40),
	IN v_TadIdNumber NVARCHAR(40)
)
LANGUAGE SQLSCRIPT
AS
BEGIN	
	SELECT T0."DocNum",
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