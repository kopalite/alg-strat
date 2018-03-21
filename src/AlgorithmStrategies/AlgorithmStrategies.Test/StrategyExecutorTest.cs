using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace AlgorithmStrategies.Test
{
    [TestClass]
    public class StrategyExecutorTest
    {
        private List<IAlgorithmStrategy<UserModel, bool>> _strategies;
        private IStrategyExecutor<UserModel, bool> _executor;

        [TestInitialize]
        public void Initialize()
        {
            _strategies = new List<IAlgorithmStrategy<UserModel, bool>>()
            {
                new IsNameUnderscoredAndDoctorStrategy(),
                new IsNameUnderscoredStrategy(),
                new IsDoctorStrategy()
            }.Activate().ToList();
            _executor = new StrategyExecutor<UserModel, bool>(_strategies);
        }

        [TestMethod]
        public void Execute_Success()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void Execute_Failure()
        {
            //Arrange
            var userModel = new UserModel { Category = "Nurses" };

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsFailure && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void Execute_Inconclusive()
        {
            //Arrange
            var userModel = new UserModel();

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(result.IsInconclusive && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscored;IsDoctor");
        }

        [TestMethod]
        public void Execute_Success_Terminating()
        {
            //Arrange
            var userModel = new UserModel { Name = "_John Smith" };
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();
            _strategies.Activate();

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void Execute_Failure_Terminating()
        {
            //Arrange
            var userModel = new UserModel { Name = "John Smith" };
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();
            _strategies.Activate();

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsFailure && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void Execute_Inconclusive_Terminating()
        {
            //Arrange
            var userModel = new UserModel();
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();
            _strategies.Activate();

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result.IsInconclusive && result.StrategyHistory == "IsNameUnderscoredAndDoctor;IsNameUnderscoredTerminating");
        }

        [TestMethod]
        public void Execute_StrategyId_Single()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };

            //Act 
            var result = _executor.Execute(userModel, 333);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.StrategyHistory == "IsDoctor");
        }

        [TestMethod]
        public void Execute_StrategyId_Chain()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };
            _strategies[1] = new IsNameUnderscoredChainedStrategy();
            _strategies.Activate();

            //Act 
            var result = _executor.Execute(userModel, 222);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.StrategyHistory == "IsNameUnderscoredChained;IsDoctor");
        }

        [TestMethod]
        public void Execute_Mix()
        {
            //Arrange
            var userModel = new UserModel { Name = "_John Smith" };
            _strategies[0] = new IsNameUnderscoredAndDoctorChainedStrategy();
            _strategies[1] = new IsNameUnderscoredTerminatingStrategy();
            _strategies.Add(new DefaultStrategy());
            _strategies.Activate();
            _executor = new StrategyExecutor<UserModel, bool>(_strategies);

            //Act 
            var result = _executor.Execute(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result.IsSuccess && result.StrategyHistory == "IsNameUnderscoredAndDoctorChained;IsDoctor;Default");
        }

        [TestMethod]
        public void Execute_TerminationType_NeverAndSuccess()
        {
            //Arrange
            var strategies = new IAlgorithmStrategy<UserModel, bool>[] { new DefaultTerminateNeverSuccessStrategy(), new DefaultTerminateAlwaysStrategy() };
            strategies.Activate();
            var executor = new StrategyExecutor<UserModel, bool>(strategies);

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.StrategyHistory == "DefaultTerminateNeverSuccess;DefaultTerminateAlways");
        }

        [TestMethod]
        public void Execute_TerminationType_NeverAndFailure()
        {
            //Arrange
            var strategies = new IAlgorithmStrategy<UserModel, bool>[] { new DefaultTerminateNeverFailureStrategy(), new DefaultTerminateAlwaysStrategy() };
            strategies.Activate();
            var executor = new StrategyExecutor<UserModel, bool>(strategies);

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.StrategyHistory == "DefaultTerminateNeverFailure;DefaultTerminateAlways");
        }

        [TestMethod]
        public void Execute_TerminationType_NeverAndInconclusive()
        {
            //Arrange
            var strategies = new IAlgorithmStrategy<UserModel, bool>[] { new DefaultTerminateNeverInconclusiveStrategy(), new DefaultTerminateAlwaysStrategy() };
            strategies.Activate();
            var executor = new StrategyExecutor<UserModel, bool>(strategies);

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.StrategyHistory == "DefaultTerminateNeverInconclusive;DefaultTerminateAlways");
        }

        [TestMethod]
        public void Execute_TerminationType_Always()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateAlwaysStrategy() }.Union(_strategies);
            var executor = new StrategyExecutor<UserModel, bool>(strategies);
            strategies.Activate();

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.StrategyHistory == "DefaultTerminateAlways");
        }

        [TestMethod]
        public void Execute_TerminationType_OnFailure()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateOnFailureStrategy() }.Union(_strategies);
            var executor = new StrategyExecutor<UserModel, bool>(strategies);
            strategies.Activate();

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.IsFailure && result.StrategyHistory == "DefaultTerminateOnFailure");
        }

        [TestMethod]
        public void Execute_TerminationType_OnSuccess()
        {
            //Arrange
            var strategies = new[] { new DefaultTerminateOnSuccessStrategy() }.Union(_strategies);
            strategies.Activate();
            var executor = new StrategyExecutor<UserModel, bool>(strategies);

            //Act
            var result = executor.Execute(new UserModel(), 0);

            //Assert
            Assert.IsTrue(result.IsSuccess && result.StrategyHistory == "DefaultTerminateOnSuccess");
        }
    }

    public class UserModel
    {
        public string Name { get; set; }

        public string Category { get; set; }
    }

    public class IsNameUnderscoredAndDoctorStrategy : AlgorithmStrategy<UserModel, bool>
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

    public class IsNameUnderscoredStrategy : AlgorithmStrategy<UserModel, bool>
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
        public override TerminationType TerminationType { get { return TerminationType.Always; } }
    }

    public class IsNameUnderscoredChainedStrategy : IsNameUnderscoredStrategy
    {
        public override string Name { get { return "IsNameUnderscoredChained"; } }
        public override int NextId { get { return 333; } }
    }

    public class IsDoctorStrategy : AlgorithmStrategy<UserModel, bool>
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

    public class DefaultStrategy : AlgorithmStrategy<UserModel, bool>
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

    public class SetableStrategy : AlgorithmStrategy<UserModel, bool>
    {
        public override int Id { get { return 10000; } }
        public override string Name { get { return "Default"; } }
    }

    public class DefaultTerminateNeverSuccessStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateNeverSuccess"; } }
        public override TerminationType TerminationType { get { return TerminationType.Never; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Success();
        }
    }

    public class DefaultTerminateNeverFailureStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateNeverFailure"; } }
        public override TerminationType TerminationType { get { return TerminationType.Never; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Failure();
        }
    }

    public class DefaultTerminateNeverInconclusiveStrategy : DefaultStrategy
    {
        public override int Priority { get { return 1; } }
        public override string Name { get { return "DefaultTerminateNeverInconclusive"; } }
        public override TerminationType TerminationType { get { return TerminationType.Never; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Inconclusive();
        }
    }

    public class DefaultTerminateAlwaysStrategy : DefaultStrategy
    {
        public override int Priority { get { return 2; } }
        public override string Name { get { return "DefaultTerminateAlways"; } }
        public override TerminationType TerminationType { get { return TerminationType.Always; } }
    }

    public class DefaultTerminateOnFailureStrategy : DefaultStrategy
    {
        public override int Priority { get { return 2; } }
        public override string Name { get { return "DefaultTerminateOnFailure"; } }
        public override TerminationType TerminationType { get { return TerminationType.OnFailure; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Failure();
        }
    }

    public class DefaultTerminateOnSuccessStrategy : DefaultStrategy
    {
        public override int Priority { get { return 2; } }
        public override string Name { get { return "DefaultTerminateOnSuccess"; } }
        public override TerminationType TerminationType { get { return TerminationType.OnSuccess; } }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            return StrategyResult<bool>.Success();
        }
    }

}
