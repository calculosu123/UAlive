﻿using Ludiq;
using Ludiq.Bolt;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;
using Lasm.Reflection;

namespace Lasm.UAlive
{
    [UnitTitle("Return [Live]")]
    [UnitCategory("Nesting")]
    [Serializable]
    public sealed class ReturnUnit : LiveUnit
    {
        [Serialize]
        [Inspectable]
        [UnitHeaderInspectable]
        public Yieldable yieldable = new Yieldable();

        [DoNotSerialize]
        [UnitPortLabelHidden]
        [UnitPrimaryPort]
        public ControlInput enter;

        [DoNotSerialize]
        [UnitPortLabelHidden]
        [UnitPrimaryPort]
        public ControlOutput exit;

        [DoNotSerialize]
        [UnitPortLabelHidden]
        public ValueInput returns;

        [Serialize] 
        public System.Type returnType;


        protected override void DefinePorts()
        {
            if (returnType != null)
            {
                if (returnType.IsEnumerable())
                {
                    enter = ControlInput("enter", OnEnterCoroutine);
                    if (yieldable != null) { if (yieldable.@yield) exit = ControlOutput("exit"); }
                }
                else
                {
                    enter = ControlInput("enter", OnEnter);
                }

                if (returnType != typeof(Void)) returns = ValueInput(returnType, "returns");
            }
        }

        private ControlOutput OnEnter(Flow flow)
        {
            SetReturn(flow);
            return null;
        }

        private ControlOutput OnEnterCoroutine(Flow flow)
        {
            if (entry.targets.Count > 0)
            {
                if (entry.targets[entry.targets.Count - 1].isCoroutine)
                {
                    Proxy.Routine(Coroutine(flow));
                    SetReturn(flow);
                    if (yieldable.@yield) return null;
                    return exit;
                }
            }

            SetReturn(flow);

            return exit;
        }

        private IEnumerator Coroutine(Flow flow)
        {
            yield return flow.GetValue<IEnumerator>(returns);
            if (yieldable.@yield) yield return Flow.New(flow.stack.AsReference(), true).Coroutine(exit);
        }

        private void SetReturn(Flow flow)
        {
            var _entry = entry as MethodInputUnit;

            if (_entry != null)
            {
                if (!_entry.declaration.IsVoid())
                {
                    Method.SetInstanceValue(entry, flow, returns);
                }
            }
        }

        public override void AfterAdd()
        {
            base.AfterAdd();

            if (graph != null) graph.onChanged += Define;
        }

        public override void BeforeRemove()
        {
            if (graph != null) graph.onChanged -= Define;

            base.BeforeRemove();
        }

    }
}