using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace AlgorithmStrategies.Test
{
    [TestClass]
    public class AlgorithmStrategiesExecutorTest
    {
        private IAlgorithmStrategy<UserModel, bool>[] _strategies;
        private IAlgorithmStrategyPerformer<UserModel, bool> _performer;

        [TestInitialize]
        public void Initialize()
        {
            _strategies = new IAlgorithmStrategy<UserModel, bool>[]
            {
                new IsNameUnderscoredDoctorStrategy(),
                new NameStartsWithUnderscoreStrategy(),
                new IsDoctorsStrategy()
            };
            _performer = new UserModelStrategyPerformer(_strategies);
        }

        [TestMethod]
        public void ByPriority_Default()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(result.Result && result == StrategyResult<bool>.Success() && result.StrategyName == "333");
        }

        [TestMethod]
        public void ByPriority_Default_Inconclusive()
        {
            //Arrange
            var userModel = new UserModel { Category = "Nurses" };

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result == StrategyResult<bool>.Inconclusive() && result.StrategyName == "333");
        }

        [TestMethod]
        public void ByPriority_Default_Terminating()
        {
            //Arrange
            var userModel = new UserModel { Category = "Doctors" };
            //_strategies[1].IsTerminating = false;

            //Act 
            var result = _performer.ByPriority(userModel, 0);

            //Assert
            Assert.IsTrue(!result.Result && result == StrategyResult<bool>.Inconclusive() && result.StrategyName == "222");
        }

        [TestMethod]
        public void ByPriority_StrategyId()
        {
        }

        [TestMethod]
        public void ByPriority_StrategyId_Terminating()
        {
        }

        [TestMethod]
        public void ByPriority_Mix()
        {
        }

        [TestMethod]
        public void ByPriority_Mix_Terminating()
        {
        }
    }

    public class UserModel
    {
        public string Name { get; set; }

        public string Category { get; set; }
    }

    public class UserModelStrategyPerformer : AlgorithmStrategyPerformer<UserModel, bool>
    {
        public UserModelStrategyPerformer(IEnumerable<IAlgorithmStrategy<UserModel, bool>> strategies) : base(strategies)
        {

        }
    }

    public class IsNameUnderscoredDoctorStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 111; } }
        public override int Priority { get { return 1; } }
        public override string Name { get { return "111"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Name != null && model.Category != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Name.StartsWith("_") && model.Category == "Doctors")
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Inconclusive();
        }
    }

    public class NameStartsWithUnderscoreStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 222; } }
        public override int Priority { get { return 2; } }
        public override string Name { get { return "2"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Name != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Name.StartsWith("_"))
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Inconclusive();
        } 
    }

    public class IsDoctorsStrategy : AlgorithmStrategyBase<UserModel, bool>
    {
        public override int Id { get { return 333; } }
        public override int Priority { get { return 3; } }
        public override string Name { get { return "333"; } }
        public override bool IsCandidate(UserModel model)
        {
            return model.Category != null;
        }
        public override StrategyResult<bool> Execute(UserModel model)
        {
            if (model.Category == "Doctors")
                return StrategyResult<bool>.Success(true);
            return StrategyResult<bool>.Inconclusive();
        }
    }

}
