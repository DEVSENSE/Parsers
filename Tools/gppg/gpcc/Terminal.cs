using System;
namespace gpcc
{
	public class Terminal : Symbol
	{
		private static int count;
		private static int max;
		public Precedence prec;
		private int n;
		public bool symbolic;
        private bool custom = false;
        private string comment;
        public string Comment { get { return comment; } }
		public override int num
		{
			get
			{
                if(custom)
                {
                    return this.n;
                }
                if (this.symbolic)
				{
					return Terminal.max + this.n;
				}
				return this.n;
			}
		}
		public Terminal(bool symbolic, string name) : base(symbolic ? name : ("'" + name.Replace("\n", "\\n") + "'"))
		{
			this.symbolic = symbolic;
			if (symbolic)
			{
				this.n = ++Terminal.count;
				return;
			}
			this.n = (int)name[0];
			if (this.n > Terminal.max)
			{
				Terminal.max = this.n;
			}
		}
        public void SetValue(int value)
        {
            this.custom = true;
            this.n = value;
        }
        public void SetComment(string comment)
        {
            this.comment = comment;
        }
        public override bool IsNullable()
		{
			return false;
		}
	}
}
