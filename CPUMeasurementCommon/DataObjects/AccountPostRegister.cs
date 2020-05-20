using System.ComponentModel.DataAnnotations;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPostRegister
    {
        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        public string Username { get; set; }
        
        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        public string Password { get; set; }

        [Required]
        [MinLength(5)]
        [MaxLength(256)]
        public string PasswordAgain { get; set; }



        [MaxLength(256)]
        public string Name { get; set; }
    }
}
