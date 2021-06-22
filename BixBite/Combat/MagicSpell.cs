using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BixBite.Combat
{
	public class MagicSpell : Skill
	{
		public EMagicType SpellMagicType = EMagicType.NONE;
		public String SpellName = String.Empty;
	}
}
