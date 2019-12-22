using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace gRPC_Demo
{
    class StudentsTempProfiles
    {
        public string matricNo { get; set; }
        public string name { get; set; }
        public string bloodGroup { get; set; }
        public string school { get; set; }
        public string programme { get; set; }
        public Image profilePics { get; set; }
        public object this[int index]
        {
            get
            {
                switch (index)
                {

                    case 0: return matricNo;
                    case 1: return name;
                    case 2: return bloodGroup;
                    case 3: return school;
                    case 4: return programme;
                    case 5: return profilePics;
                    default: return null;
                }
            }
        }
    }
}
