using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmStrategies.Test
{
    [TestClass]
    public class AlgorithmStrategiesPerformerTest
    {
        private IAlgorithmStrategy<UserModel, bool>[] _strategies;
        private IAlgorithmStrategyPerformer<UserModel, bool> _performer;

        [TestInitialize]
        public void Initialize()
        {
            _strategies = new IAlgorithmStrategy<UserModel, bool>[]
            {
                new IsNameUnderscoredAndDoctorStrategy(),
                new IsNameUnderscoredStrategy(),
                new IsDoctorStrategy()
            };
            _performer = new AlgorithmStrategyPerformer<UserModel, bool>(_strategies);
        }

        [TestMethod]
        public void ByPriority_Success()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void ByPriority_Failure()
        {
            //Arrange
            var userModel = new UserModel { Category = "Nurses" };

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsFailure && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void ByPriority_Inconclusive()
        {
            //Arrange
            var userModel = new UserModel();

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(result.IsInconclusive && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void ByPriority_Success_Terminating()
        {
            //Arrange
            var userModel = new UserModel { Name = "_John Smith" };
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void ByPriority_Failure_Terminating()
        {
            //Arrange
            var userModel = new UserModel { Name = "John Smith" };
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsFailure && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void ByPriority_Inconclusive_Terminating()
        {
            //Arrange
            var userModel = new UserModel();
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsInconclusive && result.History == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void ByPriority_StrategyId_Single()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };

            //Act 
            var result = _performer.ByPriority(userModel, 0, 333);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.History == "IsDoctor");
        }

        [TestMethod]
        public void ByPriority_StrategyId_Chain()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };
            _strategies[1] = new IsNameUnderscoredChainedStrategy();

            //Act 
            var result = _performer.ByPriority(userModel, 0, 222);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.History == "IsNameUnderscoredChained;IsDoctor");
        }

        [TestMethod]
        public void ByPriority_Mix()
        {
            //Arrange
            var userModel = new UserModel { Name = "_John Smith" };
            _strategies[0] = new IsNameUnderscoredAndDoctorChainedStrategy();
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();
            _strategies = _strategies.Union(new [] { new DefaultStrategy() }).ToArray();
            _performer = new AlgorithmStrategyPerformer<UserModel, bool>(_strategies);

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.History == "IsNameUnderscoredAndDoctorChained;IsDoctor;Default");
        }

        [TestMethod]
        public void ByPriority_TerminationType_Always()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateAlwaysStrategy() }.Union(_strategies);
            var performer = new AlgorithmStrategyPerformer<UserModel, bool>(strategies);

            //Act
            var result = performer.ByPriority(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.History == "DefaultTerminateAlways");
        }

        [TestMethod]
        public void ByPriority_TerminationType_OnFailure()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateOnFailureStrategy() }.Union(_strategies);
            var performer = new AlgorithmStrategyPerformer<UserModel, bool>(strategies);

            //Act
            var result = performer.ByPriority(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.IsFailure && result.History == "DefaultTerminateOnFailure");
        }

        [TestMethod]
        public void ByPriority_TerminationType_OnSuccess()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateOnSuccessStrategy() }.Union(_strategies);
            var performer = new AlgorithmStrategyPerformer<UserModel, bool>(strategies);

            //Act
            var result = performer.ByPriority(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.IsSuccess && result.History == "DefaultTerminateOnSuccess");
        }
    }

    public class UserModel
    {
        public string Name { get; set; }

        public string Category { get; set; }
    }

    public class IsNameUnderscoredAndDoctorStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 111; } }
        public override int Priority { get { return 10; } }
        public override string Name { get { return "IsNameUnderscoredAndDoctor"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Name != null && model.Category != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Name.StartsWith("_") && model.Category == "Doctors")
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Failure();
        }
    }

    public class IsNameUnderscoredAndDoctorChainedStrategy : IsNameUnderscoredAndDoctorStrategy
    {
        public override string Name { get { return "IsNameUnderscoredAndDoctorChained"; } }
        public override int NextId { get { return 333; } }
    }

    public class IsNameUnderscoredStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 222; } }
        public override int Priority { get { return 20; } }
        public override string Name { get { return "IsNameUnderscored"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Name != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Name.StartsWith("_"))
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Failure();
        } 
    }

    public class IsNameUnderscoredTerminatingStrategy : IsNameUnderscoredStrategy
    {
        public override string Name { get { return "IsNameUnderscoredTerminating"; } }
        public override TerminationType TerminationType { get { return TerminationType.TerminateAlways; } }
    }

    public class IsNameUnderscoredChainedStrategy : IsNameUnderscoredStrategy
    {
        public override string Name { get { return "IsNameUnderscoredChained"; } }
        public override int NextId { get { return 333; } }
    }

    public class IsDoctorStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 333; } }
        public override int Priority { get { return 30; } }
        public override string Name { get { return "IsDoctor"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Category != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Category == "Doctors")
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Failure();
        }
    }

    public class DefaultStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 444; } }
        public override int Priority { get { return 40; } }
        public override string Name { get { return "Default"; } }
        public override bool IsCandidate(UserModel model)
        {
            return true;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Success(true);
        }
    }

    public class DefaultTerminateAlwaysStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateAlways"; } }
        public override TerminationType TerminationType { get { return TerminationType.TerminateAlways; } }
    }

    public class DefaultTerminateOnFailureStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateOnFailure"; } }
        public override TerminationType TerminationType { get { return TerminationType.TerminateOnFailure; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Failure();
        }
    }

    public class DefaultTerminateOnSuccessStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateOnSuccess"; } }
        public override TerminationType TerminationType { get { return TerminationType.TerminateOnSuccess; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Success();
        }
    }

}
