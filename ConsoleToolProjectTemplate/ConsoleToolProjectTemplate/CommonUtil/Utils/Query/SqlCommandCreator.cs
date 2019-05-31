using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace System.My.CommonUtil
{
    internal class SqlCommandCreator
    {
        public string FillSelectCommand<T>(Expression<Func<T, object>>[] func)
        {
            StringBuilder builder = new StringBuilder();
            foreach (Expression<Func<T, object>> exp in func)
            {
                string field = ProcessExpressionForSelect(exp.Body);
                if (string.IsNullOrEmpty(field))
                {
                    continue;
                }

                builder.Append(field + ",");
            }
            return builder.ToString().Trim(',');
        }

        public string FillWhereCommand<T>(Expression<Func<T, object>> where, T t = default(T))
        {
            Queue<object> result = ProcessExpressionForWhere(t, Operator.None, where.Body);
            string whereCondition = ConvertQueueToCommand(result);
            whereCondition = whereCondition.Remove(whereCondition.LastIndexOf(')'));
            return whereCondition.TrimStart('(');
        }

        private string ConvertQueueToCommand(Queue<object> resultQueue)
        {
            string format = "({0})";
            int queueCount = resultQueue.Count;
            StringBuilder builder = new StringBuilder();
            while (resultQueue.Count > 0)
            {
                object unit = resultQueue.Dequeue();
                if (unit is Queue<object>)
                {
                    builder.Append(ConvertQueueToCommand((Queue<object>)unit));
                }
                else if (unit is BooleanOperation<string, Operator, object>)
                {
                    builder.Append(unit.ToString());
                }
                else
                {
                    string oper = typeof(LogicOperator).GetProperty(unit.ToString()).GetValue(null, null).ToString();
                    builder.Append(string.Format(" {0} ", oper));
                }
            }
            if (queueCount == 1)
            {
                return builder.ToString();
            }
            else
            {
                return string.Format(format, builder.ToString());
            }
        }

        private Queue<object> ProcessExpressionForWhere<T>(T t, Operator oper, Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Call:
                    MethodCallExpression callExp = exp as MethodCallExpression;
                    return ProcessMethodExpression(t, callExp);
                case ExpressionType.NewArrayInit:
                case ExpressionType.NewArrayBounds:
                    NewArrayExpression arrayExp = exp as NewArrayExpression;
                    return ProcessNewArrayExpression(t, oper, arrayExp);
                case ExpressionType.Quote:
                    UnaryExpression quoteExp = exp as UnaryExpression;
                    return ProcessExpressionForWhere(t, oper, quoteExp.Operand);
                case ExpressionType.Lambda:
                    LambdaExpression lambdaExp = exp as LambdaExpression;
                    return ProcessExpressionForWhere(t, oper, lambdaExp.Body);
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression unaryExp = exp as UnaryExpression;
                    return ProcessExpressionForWhere(t, oper, unaryExp.Operand);
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    BinaryExpression binaryExp = exp as BinaryExpression;
                    return ProcessBinaryExpression(t, oper, binaryExp);
            }
            return null;
        }

        private Queue<object> ProcessBinaryExpression<T>(T t, Operator oper, BinaryExpression binaryExp)
        {
            BooleanOperation<string, Operator, object> operation = new BooleanOperation<string, Operator, object>();
            operation.BoolOperator = ConvertExpressionTypeToOperator(binaryExp.NodeType);
            if (operation.BoolOperator == Operator.None)
            {
                return null;
            }
            ColumnAttribute attr = (ColumnAttribute)(binaryExp.Left as MemberExpression).Member.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
            if (attr != null)
            {
                operation.Left = attr.Name;
            }
            else
            {
                operation.Left = (binaryExp.Left as MemberExpression).Member.Name;
            }
            switch (binaryExp.Right.NodeType)
            {
                case ExpressionType.MemberAccess:
                    MemberExpression memberExp = binaryExp.Right as MemberExpression;
                    Type type = memberExp.Member.ReflectedType;
                    PropertyInfo property = null;
                    FieldInfo field = null;
                    if (memberExp.Member.MemberType == MemberTypes.Property)
                    {
                        property = type.GetProperty(memberExp.Member.Name);
                        if (type == typeof(T))
                        {
                            operation.Right = property.GetValue(t, null);
                        }
                        else
                        {
                            operation.Right = property.GetValue(null, null);
                        }
                    }
                    else if (memberExp.Member.MemberType == MemberTypes.Field)
                    {
                        field = type.GetField(memberExp.Member.Name);
                        if (type == typeof(T))
                        {
                            operation.Right = field.GetValue(t);
                        }
                        else
                        {
                            operation.Right = field.GetValue(null);
                        }
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case ExpressionType.Constant:
                    operation.Right = (binaryExp.Right as ConstantExpression).Value;
                    break;
            }
            Queue<object> boolExp = new Queue<object>();
            boolExp.Enqueue(operation);
            return boolExp;
        }

        private Queue<object> ProcessNewArrayExpression<T>(T t, Operator oper, NewArrayExpression arrayExp)
        {
            Queue<object> operQueue = new Queue<object>();
            int i = 0;
            foreach (Expression exp in arrayExp.Expressions)
            {
                object obj = ProcessExpressionForWhere(t, oper, exp);
                if (obj != null)
                {
                    if (i > 0)
                    {
                        operQueue.Enqueue(oper);
                    }
                    operQueue.Enqueue(obj);
                }
                i++;
            }
            return operQueue;
        }

        private Queue<object> ProcessMethodExpression<T>(T t, MethodCallExpression callExp)
        {
            Operator oper;
            if (callExp.Method.Name.Equals("WhereAnd"))
            {
                oper = Operator.And;
            }
            else if (callExp.Method.Name.Equals("WhereOr"))
            {
                oper = Operator.Or;
            }
            else
            {
                oper = Operator.None;
            }

            foreach (Expression exp in callExp.Arguments)
            {
                return ProcessExpressionForWhere(t, oper, exp);
            }
            return null;
        }

        private string ProcessExpressionForSelect(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression unaryExp = exp as UnaryExpression;
                    return ProcessExpressionForSelect(unaryExp.Operand);
                case ExpressionType.MemberAccess:
                    return ProcessMemberExpression(exp);
            }
            return string.Empty;
        }

        private string ProcessMemberExpression(Expression exp)
        {
            MemberExpression memberExp = exp as MemberExpression;
            ColumnAttribute attr = (ColumnAttribute)memberExp.Member.GetCustomAttributes(typeof(ColumnAttribute), false).FirstOrDefault();
            if (attr == null)
            {
                return memberExp.Member.Name;
            }
            else
            {
                return attr.Name;
            }
        }

        private Operator ConvertExpressionTypeToOperator(ExpressionType type)
        {
            switch (type)
            {
                case ExpressionType.Equal:
                    return Operator.Equal;
                case ExpressionType.GreaterThan:
                    return Operator.GreaterThan;
                case ExpressionType.GreaterThanOrEqual:
                    return Operator.GreaterThanOrEqual;
                case ExpressionType.LessThan:
                    return Operator.LessThan;
                case ExpressionType.LessThanOrEqual:
                    return Operator.LessThanOrEqual;
                case ExpressionType.NotEqual:
                    return Operator.NotEqual;
                default:
                    return Operator.None;
            }
        }
    }

    public class BooleanOperation<T1, Operator, T2>
    {
        public T1 Left { get; set; }
        public Operator BoolOperator { get; set; }
        public T2 Right { get; set; }

        public override string ToString()
        {
            string format = string.Empty;
            string oper = typeof(BooleanOperator).GetProperty(BoolOperator.ToString()).GetValue(null, null).ToString();
            if (Right is int
                || Right is long
                || Right is byte
                || Right is short
                || Right is double
                || Right is float)
            {
                format = "{0}{1}{2}";
            }
            else
            {
                format = "{0}{1}'{2}'";
            }
            return string.Format(format, Left, oper, Right);
        }
    }

    public class LogicOperator
    {
        public static string And { get { return "AND"; } }
        public static string Or { get { return "OR"; } }

    }

    public class BooleanOperator
    {
        public static string Equal { get { return "="; } }
        public static string GreaterThan { get { return ">"; } }
        public static string GreaterThanOrEqual { get { return ">="; } }
        public static string LessThan { get { return "<"; } }
        public static string LessThanOrEqual { get { return "<="; } }
        public static string NotEqual { get { return "<>"; } }

        public static string Like { get { return "LIKE"; } }
        public static string NotLike { get { return "NOT LIKE"; } }
        public static string Is { get { return "IS"; } }
        public static string IsNot { get { return "IS NOT"; } }
        public static string In { get { return "IN"; } }
    }

    public enum Operator
    {
        Equal,
        GreaterThan,
        GreaterThanOrEqual,
        LessThan,
        LessThanOrEqual,
        NotEqual,
        Like,
        NotLike,
        Is,
        IsNot,
        In,

        And,
        Or,
        None
    }
}
