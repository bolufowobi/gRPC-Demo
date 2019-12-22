using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Google.Protobuf;
using Grpc.Core;
using System.IO;

namespace gRPC_Server
{
    public class Students_Profiles : Students.StudentsBase
    {
        public override Task<StudentsModel> GetStudentsInfo(StudentsLookupModel request, ServerCallContext context)
        {
            StudentsModel output = new StudentsModel();
            using (DbprofilDbcontext contxt = new DbprofilDbcontext())
            {
                var row = contxt.profiles.FirstOrDefault(r => r.matricNo == request.MatricNo);
                if (row != null)
                {
                   
                    output.MatricNo = row.matricNo;
                    output.Name = row.name;
                    output.ProfilePicture = Google.Protobuf.ByteString.FromBase64(Convert.ToBase64String(row.profilePics));
                    output.BloodGroup = row.bloodGroup;
                    output.Programme = row.programme;
                    output.School = row.school;
                   

                }
                else
                {


                }
                return Task.FromResult(output);

            }
        }
        public override async Task GetAllStudentsInfo(StudentLookup request,  IServerStreamWriter<StudentsModel> responseStream, ServerCallContext context)
        {
            StudentsModel output = new StudentsModel();
            Dbrepository dbRptObj = new Dbrepository();
            var studentsList = dbRptObj.GetProfiles();
            foreach (var studentsData in studentsList)
            {
                output.MatricNo = studentsData.matricNo;
                output.Name = studentsData.name;
                output.ProfilePicture = Google.Protobuf.ByteString.FromBase64(Convert.ToBase64String(studentsData.profilePics));
                output.BloodGroup = studentsData.bloodGroup;
                output.Programme = studentsData.programme;
                output.School = studentsData.school;
                await responseStream.WriteAsync(output);
                
            }        
           
        }
        public override async Task DeleteAStudent(StudentsLookupModel request, IServerStreamWriter<StudentsModel> responseStream, ServerCallContext context)
        {
            StudentsModel output = new StudentsModel();
            using (DbprofilDbcontext contxt = new DbprofilDbcontext())
            { var row = contxt.profiles.FirstOrDefault(r => r.matricNo == request.MatricNo);
                var itemToRemove = contxt.profiles.SingleOrDefault(x => x.matricNo == request.MatricNo);

                if (itemToRemove != null)
                {
                    contxt.profiles.Remove(itemToRemove);
                    contxt.SaveChanges();
                }
                else
                { }
            }
            Dbrepository dbRptObj = new Dbrepository();
            var studentsList = dbRptObj.GetProfiles();
            foreach (var studentsData in studentsList)
            {
                output.MatricNo = studentsData.matricNo;
                output.Name = studentsData.name;
                output.ProfilePicture = Google.Protobuf.ByteString.FromBase64(Convert.ToBase64String(studentsData.profilePics));
                output.BloodGroup = studentsData.bloodGroup;
                output.Programme = studentsData.programme;
                output.School = studentsData.school;
                await responseStream.WriteAsync(output);

            }
           // return base.DeleteAStudent(request, responseStream, context);   
        }
        public  override Task<Result> AddStudent(StudentsModel request, ServerCallContext context)
        {
            Result output = new Result();
            using (DbprofilDbcontext contxt = new DbprofilDbcontext())
            {
                try
                {
                    var itemCheck = contxt.profiles.SingleOrDefault(x => x.matricNo == request.MatricNo);

                    if (itemCheck == null)
                    {
                        DbProfile dbP = new DbProfile();
                        dbP.name = request.Name;
                        dbP.matricNo = request.MatricNo;
                        dbP.programme = request.Programme;
                        dbP.school = request.School;
                        dbP.bloodGroup = request.BloodGroup;
                        dbP.profilePics = Convert.FromBase64String(request.ProfilePicture.ToBase64());
                        dbP.ClientGuid = Guid.NewGuid();
                        contxt.profiles.Add(dbP);
                        contxt.SaveChanges();
                        output.Prompt = "Success";
                    }
                    else
                    {
                        output.Prompt = "Matric No Already Exist";

                    }
                }
                catch(Exception ex)
                {
                    output.Prompt = ex.Message;
                }
                
            }
            return Task.FromResult(output);
        }
    }
}