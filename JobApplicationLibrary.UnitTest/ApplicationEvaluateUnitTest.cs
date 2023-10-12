using JobApplicationLibrary.Models;
using System;
using Moq;
using JobApplicationLibrary.Services;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
	{
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            // Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
           
        }
        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            // Arrange
            
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i=> i.IsValid(It.IsAny<string>())).Returns(true);
           
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() {  Age = 19} ,
                TechStackList = new System.Collections.Generic.List<string>() { "" }
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoRejected, appResult);
          
        }
        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>()
                {
                    "C#", "RabbitMQ", "Microservice", "Visual Studio"
                },
                YearsOfExperience = 16
            };

            // Action
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(ApplicationResult.AutoAccepted, appResult);
          
        }
        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            // Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);


            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 }
                
            };

            // Action..
            var appResult = evaluator.Evaluate(form);

            // Assert
            Assert.AreEqual(appResult,ApplicationResult.TransferredToHR );

        }



    }
}