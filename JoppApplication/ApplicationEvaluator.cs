using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using JobApplicationLibrary.Models;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary
{
    public class ApplicationEvaluator
    {
        private const int minAge = 18;
        private const int autoAcceptedYearOfExperience = 15;
        private readonly IIdentityValidator identityValidator;
        private List<string> techStackList = new() { "C#", "RabbitMQ", "Microservice", "Visual Studio" };
        

        public ApplicationEvaluator(IIdentityValidator identityValidator)
        {
           
            this.identityValidator = identityValidator;
        }
        public ApplicationResult Evaluate(JobApplication form)
        {
            if (form.Applicant is null)
                throw new ArgumentNullException();

            if (form.Applicant.Age < minAge)
                return ApplicationResult.AutoRejected;

            var validIdentity = identityValidator.IsValid(form.Applicant.IdentityNumber);
            if (!validIdentity)
                return ApplicationResult.TransferredToHR;
            

            var sr = GetTechStackSimilarityRate(form.TechStackList);

            if (sr < 25)
                return ApplicationResult.AutoRejected;

            if (sr > 75 && form.YearsOfExperience >= autoAcceptedYearOfExperience)
                return ApplicationResult.AutoAccepted;

            return ApplicationResult.AutoAccepted;
        }
        private int GetTechStackSimilarityRate(List<string> techStacks)
        {
            var matchedCount =
                 techStacks
                    .Where(i => techStackList.Contains(i, StringComparer.OrdinalIgnoreCase))
                    .Count();

            return (int)((double)matchedCount / techStackList.Count) * 100;
        }
    }
    public enum ApplicationResult
    {
        AutoRejected,
        TransferredToHR,
        TransferredToLead,
        TransferredToCTO,
        AutoAccepted
    }
}