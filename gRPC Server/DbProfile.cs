using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace gRPC_Server
{
    class DbProfile
    {
        [JsonProperty("Name")]
        public string name { get; set; }
      
        [Key]
        [JsonProperty("Matric_No")]
        public string matricNo { get; set; }

        [JsonProperty("School")]
        public string school { get; set; }
        [JsonProperty("Blood_Group")]
        public string bloodGroup { get; set; }
        [JsonProperty("Programme")]
        public string programme { get; set; }
        [JsonProperty("Profilepicture")]
        [MaxLength]
        public byte[] profilePics { get; set; }
        
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ClientGuid { get; set; }
        public object this[int index]
        {
            get
            {
                switch (index)
                {

                    case 0: return name;
                    case 1: return matricNo;
                    case 2: return school;
                    case 3: return bloodGroup;
                    case 4: return programme;
                    case 5: return profilePics;
                    default: return null;
                }
            }
        }


    }
}
