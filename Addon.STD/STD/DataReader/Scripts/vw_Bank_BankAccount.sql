ALTER VIEW "vw_Bank_BankAccount"
AS
	SELECT * FROM (
	SELECT 'VT' AS "Key",
		   T1."BankCode" AS "Code", 
		   T0."AcctName" AS "Name", 
		   T0."Account" AS "Account" ,
		   '10698' AS "SenderBankCode"
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"
	 WHERE T1."BankCode" = 'ICB'
     UNION ALL
	SELECT 'BI' AS "Key",
		   T1."BankCode" AS "Code", 
		   T0."AcctName" AS "Name", 
		   T0."Account" AS "Account" ,
		   '79202032' AS "SenderBankCode"
	  FROM DSC1 t0 
	  JOIN ODSC T1  ON T0."BankCode" = T1."BankCode"
	 WHERE T1."BankCode" = 'BIDV') T