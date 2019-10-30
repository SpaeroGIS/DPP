using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visibility.DTO
{
    class MasterResult
    {
        public IEnumerable<int>  ObservPointID{ get; set; }
        public IEnumerable<int> ObservObjectID { get; set; }
        public string RasterLayerNAME { get; set; }
         
        public bool OP { get; set; }
        public bool sumFieldOfView { get; set; }
        public bool Table { get; set; }

    }
}
