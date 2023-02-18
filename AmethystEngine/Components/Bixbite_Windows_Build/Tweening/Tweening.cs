using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace BixBite.Tweening
{

	public enum EEasingFunction
	{
		NONE,
		LINEAR,
		QUADRATIC,
		CUBIC,
		QUARTIC,
		QUINTIC,
		SINUSOIDAL,
		EXPONENTIAL,
		CIRCULAR,
		ELASTIC,
		BACK,
		BOUNCE
	}

	public enum EEasingType
	{
		NONE,
		IN,
		OUT,
		INOUT,
	}

	public class Tweening
	{
		/// <summary>
		/// Uses default frame rate of 60FPS to calculate the increase in percent
		/// </summary>
		private float _c_DefaultFrameRate = 60.0f; 
		private float _c_DefaultDeltaTime = 1.0f/60.0f;

		private double _startValue;
		private double _endValue;
		private float _totalDesiredTime;
		private float _totalElapsedTime;
		private EEasingFunction _easeFunction;
		private EEasingType _easeType;

		public bool bIsDone = false;

		public delegate void ChangeVariable_Delegate(double obj);
		public ChangeVariable_Delegate ChangeVariable_HookFunction = null;


		/// <summary>
		/// Creates a interpolation function given the desired parameters
		/// NOTE: IF YOU DON'T GIVE THIS OBJECT IT'S HOOK FUNCTION IT WON'T DO ANYTHING
		/// </summary>
		/// <param name="startValue">the starting point of the function</param>
		/// <param name="endValue">the ending point of the function</param>
		/// <param name="timeInSeconds">How long in seconds we want this function to run for.</param>
		/// <param name="easeFunc">type of math equation to use</param>
		/// <param name="easeType">type of math equation to use</param>
		public Tweening(int startValue, int endValue, float timeInSeconds, 
			EEasingFunction easeFunc= EEasingFunction.LINEAR, EEasingType easeType = EEasingType.IN)
		{
			_startValue = startValue;
			_endValue = endValue;
			_totalDesiredTime = timeInSeconds * 1000.0f;
			_totalElapsedTime = 0;
			_easeFunction = easeFunc;
			_easeType = easeType;
		}

		public void Update(GameTime gameTime)
		{
			if (_totalElapsedTime > _totalDesiredTime) bIsDone = true;
			_totalElapsedTime += gameTime.ElapsedGameTime.Milliseconds;

			// Set the hook function so we can change external values
			ChangeVariable_HookFunction?.Invoke(Interpolate(_totalElapsedTime / _totalDesiredTime));
		}

		public void SetDeltaTime(float deltaTime)
		{
			this._c_DefaultDeltaTime = deltaTime;
			this._c_DefaultFrameRate = 1.0f / deltaTime;
		}



		/// <summary>
		/// Interpolates between one value to another.
		/// </summary>
		/// <param name="startValue">Start value</param>
		/// <param name="endValue">End value</param>
		/// <param name="percent">Our progress or percentage. [0,1]</param>
		/// <returns>Interpolated value between two floats</returns>
		double Interpolate(float percent)
		{
			//return (_startValue + (_endValue - _startValue) * percent);

			switch (_easeFunction)
			{
				case EEasingFunction.LINEAR:
					return (_startValue + (_endValue - _startValue) * Easing.Linear(percent));
				case EEasingFunction.QUADRATIC:
					switch (_easeType)
					{
						case EEasingType.IN:
							return (_startValue + (_endValue - _startValue) * Easing.Quadratic.In(percent));
						case EEasingType.OUT:
							return (_startValue + (_endValue - _startValue) * Easing.Quadratic.Out(percent));
						case EEasingType.INOUT:
							return (_startValue + (_endValue - _startValue) * Easing.Quadratic.InOut(percent));
					}
					break;
				case EEasingFunction.CUBIC:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Cubic.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Cubic.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Cubic.InOut(percent));
						}
					break;
				case EEasingFunction.QUARTIC:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Quartic.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Quartic.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Quartic.InOut(percent));
						}
					break;
				case EEasingFunction.QUINTIC:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Quintic.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Quintic.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Quintic.InOut(percent));
						}
					break;
				case EEasingFunction.SINUSOIDAL:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Sinusoidal.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Sinusoidal.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Sinusoidal.InOut(percent));
						}
					break;
				case EEasingFunction.EXPONENTIAL:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Exponential.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Exponential.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Exponential.InOut(percent));
						}
					break;
				case EEasingFunction.CIRCULAR:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Circular.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Circular.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Circular.InOut(percent));
						}
					break;
				case EEasingFunction.ELASTIC:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Elastic.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Elastic.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Elastic.InOut(percent));
						}
					break;
				case EEasingFunction.BACK:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Back.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Back.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Back.InOut(percent));
						}
					break;
				case EEasingFunction.BOUNCE:
					switch (_easeType)
						{
							case EEasingType.IN:
								return (_startValue + (_endValue - _startValue) * Easing.Bounce.In(percent));
							case EEasingType.OUT:
								return (_startValue + (_endValue - _startValue) * Easing.Bounce.Out(percent));
							case EEasingType.INOUT:
								return (_startValue + (_endValue - _startValue) * Easing.Bounce.InOut(percent));
						}
					break;
			}
			// Should never get here...
			Console.WriteLine("[ERROR]: \tNO TWEENING FUNCTION TO MATCH DESIRED!!!");
			return 0.0f;
		}

	}

	public delegate double EasingFunction(float k);

	public class Easing
	{

		public static double Linear(float k)
		{
			return k;
		}

		public class Quadratic
		{
			public static double In(float k)
			{
				return k * k;
			}

			public static double Out(float k)
			{
				return k * (2f - k);
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k;
				return -0.5f * ((k -= 1f) * (k - 2f) - 1f);
			}

			public static double Bezier(float k, double c)
			{
				return c * 2 * k * (1 - k) + k * k;
			}
		};

		public class Cubic
		{
			public static double In(float k)
			{
				return k * k * k;
			}

			public static double Out(float k)
			{
				return 1f + ((k -= 1f) * k * k);
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k;
				return 0.5f * ((k -= 2f) * k * k + 2f);
			}
		};

		public class Quartic
		{
			public static double In(float k)
			{
				return k * k * k * k;
			}

			public static double Out(float k)
			{
				return 1f - ((k -= 1f) * k * k * k);
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k * k;
				return -0.5f * ((k -= 2f) * k * k * k - 2f);
			}
		};

		public class Quintic
		{
			public static double In(float k)
			{
				return k * k * k * k * k;
			}

			public static double Out(float k)
			{
				return 1f + ((k -= 1f) * k * k * k * k);
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * k * k * k * k * k;
				return 0.5f * ((k -= 2f) * k * k * k * k + 2f);
			}
		};

		public class Sinusoidal
		{
			public static double In(float k)
			{
				return 1f - Math.Cos(k * MathHelper.Pi / 2f);
			}

			public static double Out(float k)
			{
				return Math.Sin(k * MathHelper.Pi / 2f);
			}

			public static double InOut(float k)
			{
				return 0.5f * (1f - Math.Cos(MathHelper.Pi * k));
			}
		};

		public class Exponential
		{
			public static double In(float k)
			{
				return k == 0f ? 0f : Math.Pow(1024f, k - 1f);
			}

			public static double Out(float k)
			{
				return k == 1f ? 1f : 1f - Math.Pow(2f, -10f * k);
			}

			public static double InOut(float k)
			{
				if (k == 0f) return 0f;
				if (k == 1f) return 1f;
				if ((k *= 2f) < 1f) return 0.5f * Math.Pow(1024f, k - 1f);
				return 0.5f * (-Math.Pow(2f, -10f * (k - 1f)) + 2f);
			}
		};

		public class Circular
		{
			public static double In(float k)
			{
				return 1f - Math.Sqrt(1f - k * k);
			}

			public static double Out(float k)
			{
				return Math.Sqrt(1f - ((k -= 1f) * k));
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return -0.5f * (Math.Sqrt(1f - k * k) - 1);
				return 0.5f * (Math.Sqrt(1f - (k -= 2f) * k) + 1f);
			}
		};

		public class Elastic
		{
			public static double In(float k)
			{
				if (k == 0) return 0;
				if (k == 1) return 1;
				return -Math.Pow(2f, 10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * MathHelper.Pi) / 0.4f);
			}

			public static double Out(float k)
			{
				if (k == 0) return 0;
				if (k == 1) return 1;
				return Math.Pow(2f, -10f * k) * Math.Sin((k - 0.1f) * (2f * MathHelper.Pi) / 0.4f) + 1f;
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return -0.5f * Math.Pow(2f, 10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * MathHelper.Pi) / 0.4f);
				return Math.Pow(2f, -10f * (k -= 1f)) * Math.Sin((k - 0.1f) * (2f * MathHelper.Pi) / 0.4f) * 0.5f + 1f;
			}
		};

		public class Back
		{
			static double s = 1.70158f;
			static double s2 = 2.5949095f;

			public static double In(float k)
			{
				return k * k * ((s + 1f) * k - s);
			}

			public static double Out(float k)
			{
				return (k -= 1f) * k * ((s + 1f) * k + s) + 1f;
			}

			public static double InOut(float k)
			{
				if ((k *= 2f) < 1f) return 0.5f * (k * k * ((s2 + 1f) * k - s2));
				return 0.5f * ((k -= 2f) * k * ((s2 + 1f) * k + s2) + 2f);
			}
		};

		public class Bounce
		{
			public static double In(float k)
			{
				return 1f - Out(1f - k);
			}

			public static double Out(float k)
			{
				if (k < (1f / 2.75f))
				{
					return 7.5625f * k * k;
				}
				else if (k < (2f / 2.75f))
				{
					return 7.5625f * (k -= (1.5f / 2.75f)) * k + 0.75f;
				}
				else if (k < (2.5f / 2.75f))
				{
					return 7.5625f * (k -= (2.25f / 2.75f)) * k + 0.9375f;
				}
				else
				{
					return 7.5625f * (k -= (2.625f / 2.75f)) * k + 0.984375f;
				}
			}

			public static double InOut(float k)
			{
				if (k < 0.5f) return In(k * 2f) * 0.5f;
				return Out(k * 2f - 1f) * 0.5f + 0.5f;
			}
		};
	}
}