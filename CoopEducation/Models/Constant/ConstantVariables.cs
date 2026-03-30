using System.ComponentModel;

namespace CoopEducation.Models.Constant
{
    public class ConstantVariables
    {
        public enum MethodOfLogSystem
        {
            GET, POST, PUT, DELETE
        }
        public enum ResponseCode
        {
            [Description("200")]
            Success,
            [Description("500")]
            Error,
            [Description("400")]
            Incorrect,
            [Description("403")]
            Permission,
            [Description("402")]
            LicenseCardInProgress,
            [Description("409")]
            MessageDataAlreadyExists
        }
    }
}
