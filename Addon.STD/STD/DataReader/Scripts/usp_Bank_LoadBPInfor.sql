alter PROCEDURE "usp_Bank_LoadBPInfor"
(
	IN v_CardCode NVARCHAR(50)
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	SELECT T2."BankCode", T2."DflSwift", T2."DflAccount",T4."AcctName" , T3."BankName"
	  FROM OCRD T2
	  LEFT JOIN ODSC T3 ON T2."BankCode" = T3."BankCode"
	  JOIN OCRB T4 ON T4."CardCode" = t2."CardCode"
				  AND T4."BankCode" = T2."BankCode"
				  AND T4."Account" = T2."DflAccount"
	 WHERE t2."CardCode" = :v_CardCode;
END