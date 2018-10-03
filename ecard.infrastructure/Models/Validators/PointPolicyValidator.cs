//using System;
//using Oxite.Infrastructure;
//using Oxite.Validation;

//namespace Oxite.Model
//{
//    public class PointPolicyValidator : ValidatorBase<PointPolicy>
//    {
//        public PointPolicyValidator(Site site, IRegularExpressions expressions)
//            : base(site, expressions) { }

//        #region IValidator Members

//        public override ValidationState Validate(PointPolicy pointPolicy)
//        {
//            if (pointPolicy == null) throw new ArgumentNullException("pointPolicy");

//            ValidationState validationState = new ValidationState();


//            if (pointPolicy.Amount <= 0)
//            {
//                validationState.Errors.Add(CreateValidationError(pointPolicy.Amount, "Amount.ValueError", "金额必须大于 0"));
//            }
//            if (pointPolicy.Point <= 0)
//            {
//                validationState.Errors.Add(CreateValidationError(pointPolicy.Point, "Point.ValueError", "积分必须大于 0"));
//            }
//            if (string.IsNullOrEmpty(pointPolicy.DisplayName))
//            {
//                validationState.Errors.Add(CreateValidationError(pointPolicy.DisplayName, "DisplayName.RequiredError", "显示名称不能为空."));
//            }
//            else
//            {
//                if (pointPolicy.DisplayName.Length > 20)
//                    validationState.Errors.Add(CreateValidationError(pointPolicy.DisplayName, "DisplayName.MaxLengthExceededError", "显示名称度必须小于 {0}.", 20 + 1));

//            }

//            return validationState;
//        }

//        #endregion
//    }
//}