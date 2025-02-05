create VIEW "vw_PaymentUserRole" 
(
	"UserName",
	 "Role" 
) 
AS 
(
	select "UserCode" AS "UserCode",
		   "Role" AS "Role"
	  from "tbUserAssignPaymentTool"
) WITH READ ONLY

