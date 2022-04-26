using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TicoPay.Web.Models.Role
{
    public class JsTreePermission
    {
         public string id;
        public string text;
        public string icon;
        public State state;
        public List<JsTreePermission> children;

        public static JsTreePermission NewNode(string id)
        {
            return new JsTreePermission()
            {
                id = id,
                text = string.Format("Node {0}", id),
                children = new List<JsTreePermission>()
            };
        }
    }
    public class State
    {
        public bool opened = false;
        public bool disabled = false;
        public bool selected = false;

        public State(bool Opened, bool Disabled, bool Selected)
        {
            opened = Opened;
            disabled = Disabled;
            selected = Selected;
        }
    }
}