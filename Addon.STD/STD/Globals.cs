using SAPCore.Config;
using SAPCore.Form;
using SAPCore.SAP.DIAPI;
using STDLibs.Models;
using System.Linq;

namespace STDApp
{
    
    public class APIVietinBankConstrant
    {
        public static string APIVTB
        {
            get
            {
                return Configs.AddonConfigurations["APIVTB"];
            }
        } // = "https://api-uat.vietinbank.vn";
        //public static string UAT_VTB
        //{
        //} = "/vtb-api-uat/development/erp/v1";
        public static string InquiryVTB
        {
            get
            {
                return Configs.AddonConfigurations["InquiryVTB"];
            }
        }// = $"{UAT_VTB}/statement/inquiry";
         //public static string RateVTB = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
         //public static string TransferVTB = $"{UAT_VTB}//payment/transfer";
         //public static string TransferInqVTB = $"{UAT_VTB}//payment/transferInq";

        //public static string ClientID = $"fbbf1989a3ad0de68446317f5f104df0";
        //public static string ClientSecret = $"2cd7b943f4d7c5115d44b81487497ae3";
        //public static string AccountRecv = $"108004261279";
        //public static string Account = $"112000002609";

        public static string RateVTB
        {
            get
            {
                return Configs.AddonConfigurations["RateVTB"];
            }
        }// = $"vtb-api-uat/development/v1/fxrate/ForeignExchangeRate_Inq";
        public static string TransferVTB
        {
            get
            {
                return Configs.AddonConfigurations["TransferVTB"];
            }
        }// = $"{UAT_VTB}//payment/transfer";
        public static string TransferInqVTB
        {
            get
            {
                return Configs.AddonConfigurations["TransferInqVTB"];
            }
        }// = $"{UAT_VTB}//payment/transferInq";

        public static string ClientID
        {
            get
            {
                return Configs.AddonConfigurations["ClientIDVTB"];
            }
        }//            = $"fbbf1989a3ad0de68446317f5f104df0";
        public static string ClientSecret
        {
            get
            {
                return Configs.AddonConfigurations["ClientSecretVTB"];
            }
        }// = $"2cd7b943f4d7c5115d44b81487497ae3";

        public static string USER_APPROVE
        {
            get
            {
                return Configs.AddonConfigurations["USER_APPROVE"];
            }
        }
        public static string USER_CREATE
        {
            get
            {
                return Configs.AddonConfigurations["USER_CREATE"];
            }
        }
        public static string ProviderId
        {
            get
            {
                return Configs.AddonConfigurations["ProviderId"];
            }
        }
        public static string MerchantId
        {
            get
            {
                return Configs.AddonConfigurations["MerchantId"];
            }
        }
        public static string ClientIP
        {
            get
            {
                return Configs.AddonConfigurations["ClientIP"];
            }
        }
    }
    public class APIBIDVConstrant
    {
        public static string APILink
        {
            get
            {
                return Configs.AddonConfigurations["APILinkBIDV"];
            }
        }// = "https://www.bidv.net:9303";
         // public static string UAT_BIDV = "/bidvorg/service";
        public static string InquiryBIDV
        {
            get
            {
                return Configs.AddonConfigurations["InquiryBIDV"];
            }
        }// = $"{UAT_BIDV}/open-banking/inquire-account-transaction/v1";
        //public static string TransferBIDV = "";
        public static string AuthenBIDV
        {
            get
            {
                return Configs.AddonConfigurations["AuthenBIDV"];
            }
        }// = $"{UAT_BIDV}/ibank-oauth/oauth2/token";

        public static string ChannelBIDVAPI
        {
            get
            {
                return Configs.AddonConfigurations["ChannelBIDVAPI"];
            }
        }// = "IERP";
        public static string UserAgentIDVAPI
        {
            get
            {
                return Configs.AddonConfigurations["UserAgentIDVAPI"];
            }
        }// = "IERP";
        public static string SymmetricKey
        {
            get
            {
                return Configs.AddonConfigurations["SymmetricKey"];
            }
        }// = "2094508740466af766392a869e92ac81229e15ec3e75b0476d8fbdbc9d924f65";
        public static string ClientID
        {
            get
            {
                return Configs.AddonConfigurations["ClientIDBIDV"];
            }
        }// = $"df616d171dae4974042d4d95577c05de";
        public static string ClientSecret
        {
            get
            {
                return Configs.AddonConfigurations["ClientSecretBIDV"];
            }
        }// = $"9adc6d5f750dcdfc898c74aec2d0e0c7";
        public static string URL_Redirect
        {
            get
            {
                return Configs.AddonConfigurations["URL_Redirect"];
            }
        }// = $"https://azstvntstsapb1db:50000/b1s/v1/BIDV_TEST";

        public static string TransferBIDV
        {
            get
            {
                return Configs.AddonConfigurations["TransferBIDV"];
            }
        }// = $"{UAT_BIDV}/open-banking/domestic-payment/v1";
    }

    public class APIPayooConstant
    {
        public static string APILink
        {
            get
            {
                return Configs.AddonConfigurations["APILinkPY"];
            }
        }// = "https://bizsandbox.payoo.com.vn";
        public static string SettlementTransactionsLink
        {
            get
            {
                return Configs.AddonConfigurations["SettlementTransactionsLink"];
            }
        }// = "/BusinessRestAPI.svc/GetSettlementTransactions";
        public static string APIUsername
        {
            get
            {
                return Configs.AddonConfigurations["APIUsernamePY"];
            }
        }// = "SB_Stada_BizAPI";
        public static string APIPassword
        {
            get
            {
                return Configs.AddonConfigurations["APIPasswordPY"];
            }
        }// = "9qW0k9/5khmSqdOE";
        public static string APISignature
        {
            get
            {
                return Configs.AddonConfigurations["APISignaturePY"];
            }
        }// = "U5E0ykWrsQSqkjS6xR+Qnj3cbuNUi8YgSC2r6/BcXrpdcjOx6XYh7XNjQf806Yay";

        public static string ChecksumKey
        {
            get
            {
                return Configs.AddonConfigurations["ChecksumKeyPY"];
            }
        }// = "NzlkNTJmZDBhMjcyNDM0MjBkZWQ4NDI0ODNjYjY2YTI=";
    }

    public class GlobalsConfig : SysGlobals
    {
        public AddonUserForm PaymentFormInfo;
        public AddonUserForm PaymentReviewFormInfo;
        public AddonUserForm PaymentDetailFormInfo;
        public AddonUserForm PaymentApproveFormInfo;
        public AddonUserForm FilterFormInfo;

        private static GlobalsConfig instance;

        private GlobalsConfig()
        {
            AddonName = "Stada";
            Version = "1.0";
            FlagVersion = 0;
            CusPMFolderID = "IntegrationCustomize";
            CusPMFolderDesc = "Nhóm chức năng Tích hợp";
            ParentMenuID = "43520";
            
        }

        public static GlobalsConfig Instance
        {
            get
            {
                if (instance == null)
                    instance = new GlobalsConfig();
                return instance;
            }
        }
    }

    public class STRING_CONTRANTS
    {
        public static string StartConfigUDF = "Cấu hình UDF";
        public static string ConfigUDFNotice = "Tạo UDF: {0} cho {1}";
        public static string EndConfigUDF = "Cấu hình UDF hoàn tất";

        public static string Notice_Version = "Version hiện tại đang cũ, hãy nâng cấp Version mới";

        public static string Notice_LoadData = "Đang tải dữ liệu";
        public static string Notice_EndLoadData = "Tải dữ liệu hoàn tất";
        public static string Error_LoadData = "Tải dữ liệu bị lỗi: {0}";

        public static string Notice_CreatePayment = "Đang tạo đơn thanh toán";
        public static string Notice_LoadDataBefAdd = "Vui lòng tải dữ liệu trước khi thêm mới";
        public static string Notice_EndCreatePayment = "Hoàn tất quá trình tạo thanh toán";
        public static string Error_CreatePayment = "Quà trình tạo đơn thanh toán lỗi: {0}";
        public static string Notice_NoDataToAdd = "Không có thông tin được thêm";

        public static string Notice_UpdatePayment = "Đang cập nhật đơn thanh toán";
        public static string Notice_EndUpdatePayment = "Hoàn tất quá trình cập nhật thanh toán";

        public static string Notice_UpdatePaymentSuccess = "Cập nhật thành công";
        public static string Notice_CanNotloadDraft = "Không thể tải đơn nháp";
        public static string Notice_EndAction = "{0} thành công {1}/{2} đơn thanh toán, và {3}/{2} không thành công";

        public static string Validate_DateSelectNull = "Bộ lọc [Từ Ngày]/[Đến Ngày] phải có giá trị đúng";
        public static string Validate_FromDateEarlyToDate = "Chọn bộ lọc [Từ ngày] phải trước bộ lọc [Đến ngày]";


        public static string Validate_TransDateSelectNull = "Bộ lọc Ngày phải có giá trị đúng";

        public static string NoData = "Không có dữ liệu";
        public static string NoDataCheck = "Chưa chọn dữ liệu";
        public static string NoChooseCashFlow = "Không chọn dòng tiền";
        public static string AllOptionDesc = "Tất cả";
        public static string AllOption = "All";

        public static string Action_Create = "Tạo";

        public static string DifferenceCurrency = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Đơn Vị Tiền Tệ.";
        public static string NoCurrency = "Không có thông tin Đơn Vị Tiền Tệ.";
        public static string DifferenceBank = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Ngân hàng.";
        public static string NoBank = "Không có thông tin Ngân hàng.";
        public static string DifferenceCashFlow = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Dòng tiền.";
        public static string NoCashFlow = "Không có thông tin Dòng tiền.";
        public static string DifferenceAccount = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về Tài khoản tiền mặt.";
        public static string NoAccount = "Không có thông tin Tài khoản tiền mặt.";
        public static string WrongFormatCurrency = "Vui lòng nhập đúng định dạng giá tri tiền";
        public static string OverTotalPayment = "Cần thanh toán ít hơn hay bằng số tiền phải trả [Số Tiền chưa Thanh toán]";
        public static string PaymentZero = "Số tiền thanh toán phải lớn hơn 0";
        public static string NoCFlow = "Cần phải chọn dòng tiền cho đơn thanh toán";
        public static string NoRate = "Vui lòng nhập tỉ giá";
        public static string DifferenceRate = "Không thể tạo phiếu thanh toán nhiều hóa đơn khác nhau về  tỉ giá.";


        public static string CanNotGenerateKey = "Không thể tạo key cho đơn thanh toán, vui lòng thử lại";
        public static string CanNotPayment = "Không thể chọn chứng từ có Số Tiền Thanh Toán bằng 0";
        public static string CanNotConnectDIAPI = "Không thể kết nối DI API, vui lòng thử lại";

        public static string PaymentType_PC = "Phiếu Chi";
        public static string PaymentType_PT = "Phiếu Thu";
        public static string PaymentType_UC = "Ủy Nhiệm Chi";

        public static string PaymentStatus_Pending = "Yêu cầu thanh toán";
        public static string PaymentStatus_Viewed = "Đề nghị thanh toán";
        public static string PaymentStatus_Approved = "Đã phê duyệt";
        public static string PaymentStatus_Rejected = "Từ chối";
        public static string PaymentStatus_Generated = "Tạo thành công";
        public static string PaymentStatus_All = "Tất cả";

        public static string Title_CustomerCode = "Mã Khách hàng";
        public static string Title_Customer = "Khách hàng";
        public static string Title_CustomerName = "Tên Khách hàng";
        public static string Title_VendorCode = "Mã Nhà cung cấp";
        public static string Title_Vendor= "Nhà cung cấp";
        public static string Title_VendorName = "Tên Nhà cung cấp";
        public static string Title_Choose = "Chọn";
        public static string Title_DocNum = "Số chứng từ";
        public static string Title_InvCode = "Số Hóa Đơn";
        public static string Title_DocDate = "Ngày Chứng từ";
        public static string Title_DueDate = "Hạn Thanh Toán";
        public static string Title_PostingDate = "Ngày Thanh Toán";
        public static string Title_Currency = "Đơn vị tiền tệ";
        public static string Title_Remark = "Ghi chú";
        public static string Title_InsTotal = "Số tiền hóa đơn";
        public static string Title_InsTotalFC = "Số Tiền Hóa Đơn (Ngoại tệ)";
        public static string Title_SumTotal = "Số tiền thanh toán";
        public static string Title_SumTotalFC = "Số Tiền thanh toán (Ngoại tệ)";
        public static string Title_MustPay = "Số tiền chưa thanh toán";
        public static string Title_PayAmount = "Số tiền thanh toán";
        public static string Title_Rate = "Tỉ giá ngoại tệ";
        public static string Title_Account = "Tài khoản";
        public static string Title_MustPayAsCash = "Số Tiền Thanh toán (Cash)";
        public static string Title_MustPayAsBank = "Số Tiền Thanh toán (Bank)";
        public static string Title_PaymentKey = "Mã thanh toán";
        public static string Title_PaymentType = "Loại thanh toán";
        public static string Title_PaymentDocEntry = "Số TT";
        public static string Title_PaymentDraftEntry = "Số TT nháp";
        public static string Title_PaymentDocNum = "Số chứng từ TT";
        public static string Title_PaymentDocDate = "Ngày chứng từ TT";
        public static string Title_CreateBy = "Người Tạo";
        public static string Title_Status = "Trạng Thái";
        public static string Title_Bank = "Ngân hàng";
        public static string Title_BankAccount = "Tài khoản Ngân hàng";
        public static string Title_FeeAccount = "Tài khoản chịu phí";
        public static string Title_CFlow = "Dòng tiền";

        public static string Title_Content = "Nội dung thanh toán";
        public static string Title_SAPStatus= "Trang thái từ SAP";
        public static string Title_BankStatus = "Trang thái từ API";
        public static string Title_Message = "Nội dung";
    }
}
