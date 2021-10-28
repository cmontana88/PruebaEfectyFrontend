using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PruebaEfectyFrontend.Models
{
    public class TipoDocumentoViewModel
    {
        public int Id { get; set; }

        [StringLength(20)]
        public string Valor { get; set; }
    }
}
