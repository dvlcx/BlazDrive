using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazDrive.Models.OutputModels
{
    public class OutputFile
    {
        public string FileName { get; set; }
        public byte[] File { get; set; }
        public Guid UserId { get; set; }
    }
}