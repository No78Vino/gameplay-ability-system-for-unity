using UnityEngine;

namespace GAS.Runtime
{
    /// <summary>
    /// 这是一个非常通用的数值计算器，用于计算各种属性的最终值。
    /// <para>
    /// 计算公式:
    ///     最终值 = (基础值 + 基础附加值) * (1 + 累加加成) * 累乘加成 * (1 - 最大值惩罚)
    /// </para> 
    /// </summary>
    public class AttributeValueProcessor
    {
        public float Value { get; private set; }
        public float MinValue { get; private set; }
        public float MaxValue { get; private set; }

        public delegate float OnPreValueChange(AttributeValueProcessor processor, float oldValue, float newValue);

        public delegate void OnPostValueChange(AttributeValueProcessor processor, float oldValue, float newValue);

        private OnPreValueChange _onPreValueChange;
        private event OnPostValueChange _onPostValueChange;

        private AttributeBase Base { get; }
        private AttributeBase BaseAdditiveBonus { get; }
        private AttributeBase AdditiveBonus { get; }
        private AttributeBase MultiplicativeBonus { get; }
        private AttributeBase MaxValuePenalty { get; }

        /// <summary>
        /// 计算公式:
        ///     最终值 = (基础值 + 基础附加值) * (1 + 累加加成) * 累乘加成 * (1 - 最大值惩罚)
        /// </summary>
        /// <param name="base">基础值</param>
        /// <param name="baseAdditiveBonus">基础附加值：默认0，应使用加法来修改。
        ///     一般用于增加固定数值的装备加成: 比如增加10点攻击力。
        ///     参考英雄联盟中的额外攻击力，若不需要区分基础值和基础附加值，无视此参数。
        /// </param>
        /// <param name="additiveBonus">累加加成：默认0，应使用加法来修改，25%加成对应的修改参数应该为0.25。
        ///     将所有的累加加成求和之后作为一个加成值(1 + a1 + a2 + ... + an), 它们的收益是会"稀释"的。
        ///     一般用于增加百分比的装备加成: 比如增加25%伤害。
        /// </param>
        /// <param name="multiplicativeBonus">累乘加成：默认1(需要你自己设置)，应使用乘法来修改，25%加成对应的修改参数应该为1.25。
        ///     每个技能的加成值都以乘法来计算(m1 * m2 * ... * mn), 它们的收益不会被"稀释"，因此累乘加成效果很强大，但同时也是游戏后期数值容易爆炸的根源! 具体使用累加还是累乘取决于设定。
        ///     一般用于技能/天赋/被动加成: 比如增加5%总伤害(有些游戏会用"总增"这个词来描述此类效果)。
        /// </param>
        /// <param name="maxValuePenalty">最大值惩罚：默认0, 取值范围一般为[0~1]. 注意：25%惩罚对应的修改参数应该为0.25，而不是-0.25。
        ///     仅在需要惩罚取最大值时使用，参考英雄联盟中的减少移动速度效果, 当被多个debuff减少移动速度时, 只适用减速幅度最大的效果。
        ///     该属性应该是MaxValueOnly的，即只有最大值才会生效，不会叠加。
        /// </param>
        /// <param name="minValue">最小值</param>
        /// <param name="maxValue">最大值</param>
        public AttributeValueProcessor(
            AttributeBase @base,
            AttributeBase baseAdditiveBonus = null,
            AttributeBase additiveBonus = null,
            AttributeBase multiplicativeBonus = null,
            AttributeBase maxValuePenalty = null,
            float minValue = float.MinValue,
            float maxValue = float.MaxValue)
        {
            Base = @base;
            BaseAdditiveBonus = baseAdditiveBonus;
            AdditiveBonus = additiveBonus;
            MultiplicativeBonus = multiplicativeBonus;
            MaxValuePenalty = maxValuePenalty;

            MinValue = minValue;
            MaxValue = maxValue;

            Value = CalculateValue();

            RegisterAttributeChangedListen();
        }

        public void Dispose()
        {
            _onPreValueChange = null;
            _onPostValueChange = null;
            UnRegisterAttributeChangedListen();
        }

        public void SetMinValue(float minValue)
        {
            MinValue = minValue;
            Value = CalculateValue();
        }

        public void SetMaxValue(float maxValue)
        {
            MaxValue = maxValue;
            Value = CalculateValue();
        }

        public void SetMinMaxValue(float minValue, float maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Value = CalculateValue();
        }

        /// <summary>
        /// 可以使用这个来做自定义的伤害钳制函数, 甚至当成自定义的数值计算函数
        /// 参考英雄联盟中移动速度的计算, 当移动速度超过或低于一定数值会被修正: https://leagueoflegends.fandom.com/wiki/Movement_speed
        /// </summary>
        public void SetPreValueChangeCallback(OnPreValueChange action)
        {
            _onPreValueChange = action;
        }

        public void RegisterPostValueChange(OnPostValueChange action)
        {
            _onPostValueChange += action;
        }

        public void UnregisterPostValueChange(OnPostValueChange action)
        {
            _onPostValueChange -= action;
        }

        private void RegisterAttributeChangedListen()
        {
            Base.RegisterPostCurrentValueChange(OnAttributeChanged);
            BaseAdditiveBonus?.RegisterPostCurrentValueChange(OnAttributeChanged);
            AdditiveBonus?.RegisterPostCurrentValueChange(OnAttributeChanged);
            MultiplicativeBonus?.RegisterPostCurrentValueChange(OnAttributeChanged);
            MaxValuePenalty?.RegisterPostCurrentValueChange(OnAttributeChanged);
        }

        private void UnRegisterAttributeChangedListen()
        {
            Base.UnregisterPostCurrentValueChange(OnAttributeChanged);
            BaseAdditiveBonus?.UnregisterPostCurrentValueChange(OnAttributeChanged);
            AdditiveBonus?.UnregisterPostCurrentValueChange(OnAttributeChanged);
            MultiplicativeBonus?.UnregisterPostCurrentValueChange(OnAttributeChanged);
            MaxValuePenalty?.UnregisterPostCurrentValueChange(OnAttributeChanged);
        }

        private void OnAttributeChanged(AttributeBase attribute, float attrOldValue, float attrNewValue)
        {
            var oldValue = Value;
            var newValue = CalculateValue();

            newValue = Mathf.Clamp(newValue, MinValue, MaxValue);
            if (_onPreValueChange != null)
            {
                newValue = _onPreValueChange(this, oldValue, newValue);
            }

            if (Mathf.Approximately(oldValue, newValue))
            {
                return;
            }

            Value = newValue;
            _onPostValueChange?.Invoke(this, oldValue, newValue);
        }

        private float CalculateValue()
        {
            return CalculateTotalValue(Base, BaseAdditiveBonus,
                AdditiveBonus, MultiplicativeBonus,
                MaxValuePenalty);
        }

        public static float CalculateTotalValue(
            AttributeBase @base,
            AttributeBase baseAdditiveBonus = null,
            AttributeBase additiveBonus = null,
            AttributeBase multiplicativeBonus = null,
            AttributeBase maxValuePenalty = null)
        {
            var totalValue = @base.CurrentValue;

            if (baseAdditiveBonus != null)
            {
                totalValue += baseAdditiveBonus.CurrentValue;
            }

            // 如果基础值为0, 计算就没有意义了, 可以忽略
            if (totalValue != 0)
            {
                if (additiveBonus != null)
                {
                    totalValue *= 1 + additiveBonus.CurrentValue;
                }

                if (multiplicativeBonus != null)
                {
                    totalValue *= multiplicativeBonus.CurrentValue;
                }

                if (maxValuePenalty != null)
                {
                    totalValue *= 1 - maxValuePenalty.CurrentValue;
                }
            }

            return totalValue;
        }
    }
}