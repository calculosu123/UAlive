﻿using Ludiq.Bolt;
using Ludiq;
using System.Collections.Generic;
using Lasm.Reflection;

namespace Lasm.UAlive
{
    [UnitTitle("Switch On Type [Live]")]
    [UnitCategory("Flow")]
    public class SwitchOnTypeNext : SwitchNext
    {
        [Serialize]
        [Inspectable]
        public List<System.Type> cases = new List<System.Type>();
        [DoNotSerialize]
        [UnitPrimaryPort(showLabel: false)]
        public ValueInput value;
        [UnitPortLabelHidden]
        [DoNotSerialize]
        public List<ControlOutput> _cases = new List<ControlOutput>();

        protected override void DefinePorts()
        {
            base.DefinePorts();

            _cases.Clear();

            value = ValueInput<System.Type>("value");

            for (int i = 0; i < cases.Count; i++)
            {
                _cases.Add(ControlOutput(cases[i].Name));
            }
        }

        public override void OnCase(Flow flow)
        {
            var _flow = Flow.New(flow.stack.ToReference(), true);
            var type = flow.GetValue(value) as System.Type;
            _flow.Invoke(controlOutputs[type.Name]);
        }
    }
}