CREATE PROCEDURE "usp_PaymentListReport"
(
	IN v_FromDate DATE,
	IN v_ToDate DATE
)
LANGUAGE SQLSCRIPT
AS
BEGIN

	SELECT * 
	  FROM (
			SELECT COALESCE("U_PaymentKey", '') AS "PaymentKey",
			       t0."CardCode", 
			       T0."DocNum",		
			       T0."DocEntry" AS "PaymentEntry",
			       T0."DocDate",   
			       T0."DocCurr",  
		           CASE WHEN COALESCE(T0."DocCurr", 'VND') = 'VND' THEN T0."TrsfrSum" + T0."CashSum"
				   	 	ELSE "TrsfrSumFC" + "TrsfrSumFC"
				   	END AS "Amount"
			  FROM OVPM T0
			  JOIN OUSR T1 ON T0."UserSign" = T1."USERID"
			 WHERE T0."DocDate" BETWEEN :v_FromDate AND :v_ToDate
			   AND COALESCE("U_PaymentKey", '') <> ''
			   AND "Canceled" = 'N'
  		 ) T
  ORDER BY "PaymentKey";
	
END;
 
