create PROCEDURE "usp_PaymentListReportDetail"
(
	IN v_PaymentNumber INTEGER
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	SELECT T4."CardCode",
			   T2."CardName",
			   T0."DocNum",
			   T0."DocDate", 
			   T1."DueDate",
			   CASE WHEN  T4."DocCurr" = 'VND' THEN T1."InsTotal"
			   		ELSE T1."InsTotalFC"
			   	END AS "InsTotal",
			   CASE WHEN T4."DocCurr" = 'VND' THEN T1."InsTotal" - T1."PaidToDate"
			   		ELSE T1."InsTotalFC" - T1."PaidFC" 
			   	END AS "MustPay",
			   T4."DocCurr" AS "DocCur",
			   COALESCE(T3."SumApplied", 0) AS "SumAmout",
			   COALESCE(T3."AppliedFC", 0) AS "SumAmoutFC" 
		  FROM OVPM T4
		  LEFT JOIN VPM2 T3 ON T3."DocNum" = T4."DocEntry"
		  				   AND T3."InvType" = 18
		  JOIN OPCH T0 ON T3."DocEntry" = T0."DocEntry"				  
		  LEFT JOIN PCH6 T1 ON T0."DocEntry" = T1."DocEntry"
		  LEFT JOIN OCRD T2 ON T4."CardCode" = T2."CardCode"
		 WHERE T4."DocNum" = :v_PaymentNumber 
   		   AND T4."Canceled" = 'N';
END;
