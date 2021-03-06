﻿using System.Collections.Generic;
using Lasm.UAlive;
using Lasm.UAlive.Generation;
using Ludiq.Bolt;

[assembly: RegisterCodeGenerator(typeof(SwitchOnCharNext), typeof(SwitchOnCharNextGenerator))]

namespace Lasm.UAlive
{
    public class SwitchOnEnumNextGenerator : LiveUnitGenerator<SwitchOnEnumNext>
    {
        public SwitchOnEnumNextGenerator(SwitchOnEnumNext unit) : base(unit)
        {
        }

        public override List<string> usingStatements => new List<string>() { "System" };

        public override string GenerateControlInput(ControlInput input, int indent)
        {
            var output = string.Empty;

            var items = liveUnit.cases;

            string value = null;

            var enumConnection = liveUnit._enum.connection;
            var enumSource = enumConnection.source;

            if (enumConnection != null && liveUnit._enum.connection.sourceExists)
            {
                value = enumSource.unit.CodeGenerator().GenerateValueOutput(enumSource, 0);
            }

            if (string.IsNullOrEmpty(value))
            {
                return output;
            }

            output += CodeBuilder.Indent(indent) + "switch(" + value + ")" + "\n";
            output += CodeBuilder.Indent(indent) + "{" + "\n";

            var count = 0;

            foreach (object @case in items)
            {
                var caseConnection = liveUnit.cases[count].connection;
                var caseDestination = caseConnection.destination;

                output += CodeBuilder.Indent(indent + 1) + "case " + @case + " :" + "\n";
                output += caseConnection != null ? caseDestination.unit.CodeGenerator().GenerateControlInput(caseDestination, indent + 2) : string.Empty;
                output += "\n";
                output += CodeBuilder.Indent(indent + 2) + "break;" + "\n" + (count == items.Count - 1 ? string.Empty : "\n");
                count++;
            }

            output += CodeBuilder.Indent(indent) + "}\n";

            var nextConnection = liveUnit.next.connection;
            var nextDestination = nextConnection.destination;

            if (liveUnit.next.connection != null) output += "\n" + nextDestination.unit.CodeGenerator().GenerateControlInput(nextDestination, indent);

            return output;
        }

        public override string GenerateValueOutput(ValueOutput output, int indent)
        {
            return string.Empty;
        }
    }
}
