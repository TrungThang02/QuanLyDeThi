using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuanLyDeThi.Models
{
    public class ResetPassword
    {
        [Required(ErrorMessage = "Bắt buộc phải nhập password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu không trùng khớp")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string ResetCode { get; set; }
    }
}