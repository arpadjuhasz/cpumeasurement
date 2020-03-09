using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CPUMeasurementCommon.DataObjects
{
    public class AccountPost
    {
        [Required]
        [Range(8, 256)]
        public string Username { get; set; }
        
        [Required]
        [Range(8, 256)]
        public string Password { get; set; }

        [Required]
        [Range(8, 256)]
        public string PasswordAgain { get; set; }



        [Range(0, 256)]
        public string Name { get; set; }
    }
}
