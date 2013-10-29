using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Util;
using Android.Views;
using Android.Widget;
using Java.Lang;
using Java.Util;
using Math = System.Math;
using Object = Java.Lang.Object;

namespace com.kobynet.flowtext
{
	public class FlowTextViewSharp : RelativeLayout
	{
			public FlowTextViewSharp(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
			{
					init(context);
			}

			public FlowTextViewSharp(Context context, IAttributeSet attrs) : base(context, attrs)
			{
					init(context);
			}

			public FlowTextViewSharp(Context context) : base(context)
			{
					init(context);
			}


            private Color mColor = Color.Black;
			private int pageHeight = 0;

			public virtual Color Color
			{
				set
				{
						this.mColor = value;
    
    
    
						if (mTextPaint != null)
						{
								mTextPaint.Color = mColor;
						}
    
						foreach (TextPaint paint in mPaintHeap)
						{
								paint.Color = mColor;
						}
    
						this.Invalidate();
				}
			}

			private void init(Context context)
			{

					mTextPaint = new TextPaint(PaintFlags.AntiAlias) {
					    Density = Resources.DisplayMetrics.Density,
					    TextSize = mTextsize,
					    Color = Color.Black
					};

			    mLinkPaint = new TextPaint(PaintFlags.AntiAlias) {
					    Density = Resources.DisplayMetrics.Density,
					    TextSize = mTextsize,
					    Color = Color.Blue,
					    UnderlineText = true
					};

			    this.SetBackgroundColor (Color.Transparent);

                    this.SetOnTouchListener (new OnTouchListenerAnonymousInnerClassHelper (this));    
                //this.OnTouchListener = new OnTouchListenerAnonymousInnerClassHelper(this);

			}

			private class OnTouchListenerAnonymousInnerClassHelper : IOnTouchListener
			{
				private readonly FlowTextViewSharp _outerInstance;

				public OnTouchListenerAnonymousInnerClassHelper(FlowTextViewSharp outerInstance)
				{
					_outerInstance = outerInstance;
					distance = 0;
				}

				internal double distance;

				internal float x1, y1, x2, y2;

				public bool OnTouch(View v, MotionEvent e)
				{

						var event_code = e.Action;

						if (event_code == MotionEventActions.Down)
						{
								distance = 0;
								x1 = e.GetX();
								y1 = e.GetY();
						}

                        if (event_code == MotionEventActions.Down)
						{
                            x2 = e.GetX ();
                            y2 = e.GetY ();
							distance = getPointDistance(x1, y1, x2, y2);
						}

						if (distance < 10)
						{

								if (event_code == MotionEventActions.Up)
								{

										_outerInstance.onClick(e.GetX(), @e.GetY());
								}

								return true;
						}
						else
						{
								return false;
						}
				}

			    public void Dispose ()
			    {
			        
			    }

			    public IntPtr Handle { get; private set; }
			}

			private static double getPointDistance(float x1, float y1, float x2, float y2)
			{
					double dist = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
					return dist;

			}

			private TextPaint mTextPaint;
			private TextPaint mLinkPaint;

			private int mTextsize = 20;
			public virtual int TextSize
			{
				set
				{
						this.mTextsize = value;
						mTextPaint.TextSize = mTextsize;
						mLinkPaint.TextSize = mTextsize;
						Invalidate();
				}
			}

			private Typeface typeFace;
			public virtual Typeface Typeface
			{
				set
				{
						typeFace = value;
						mTextPaint.SetTypeface (typeFace);
						mLinkPaint.SetTypeface (typeFace);
						Invalidate();
				}
			}

			private int mDesiredHeight = 100; // height of the whole view

			private float mSpacingMult = 1.0f;
			private float mSpacingAdd = 0.0f;

			private float mViewWidth;

	    internal class Area
			{
				private readonly FlowTextViewSharp outerInstance;

				public Area(FlowTextViewSharp outerInstance)
				{
					this.outerInstance = outerInstance;
				}

					internal float x1;
					internal float x2;
					internal float width;
			}


			private List<Box> mLineboxes = new List<FlowTextViewSharp.Box>();
			private List<Area> mAreas = new List<FlowTextViewSharp.Area>();

			private void onClick(float x, float y)
			{

					foreach (HtmlLink link in mLinks)
					{
							float tlX = link.xOffset;
							float tlY = link.yOffset;
							float brX = link.xOffset + link.width;
							float brY = link.yOffset + link.height;

							if (x > tlX && x < brX)
							{
									if (y > tlY && y < brY)
									{
											// collision
											onLinkClick(link.url);
											return;
									}
							}

					}
			}

			private IOnLinkClickListener mOnLinkClickListener;

			public virtual IOnLinkClickListener OnLinkClickListener
			{
				set
				{
						this.mOnLinkClickListener = value;
				}
			}

			public interface IOnLinkClickListener
			{
					void onLinkClick(string url);
			}

			private void onLinkClick(string url)
			{
					if (mOnLinkClickListener != null)
					{
						mOnLinkClickListener.onLinkClick(url);
					}
			}

			private Line getLine(float lineYbottom, int lineHeight)
			{

					Line line = new Line(this);
					line.leftBound = 0;
					line.rightBound = mViewWidth;

					float lineYtop = lineYbottom - lineHeight;

					mAreas.Clear();
					mLineboxes.Clear();

					foreach (Box box in boxes)
					{

							if (box.topLefty > lineYbottom || box.bottomRighty < lineYtop)
							{

							}
							else
							{

									Area leftArea = new Area(this);
									leftArea.x1 = 0;

									foreach (Box innerBox in boxes)
									{
											if (innerBox.topLefty > lineYbottom || innerBox.bottomRighty < lineYtop)
											{

											}
											else
											{
													if (innerBox.topLeftx < box.topLeftx)
													{
															leftArea.x1 = innerBox.bottomRightx;
													}
											}
									}

									leftArea.x2 = box.topLeftx;
									leftArea.width = leftArea.x2 - leftArea.x1;

									Area rightArea = new Area(this);
									rightArea.x1 = box.bottomRightx;
									rightArea.x2 = mViewWidth;

									foreach (Box innerBox in boxes)
									{
											if (innerBox.topLefty > lineYbottom || innerBox.bottomRighty < lineYtop)
											{

											}
											else
											{
													if (innerBox.bottomRightx > box.bottomRightx)
													{
															rightArea.x2 = innerBox.topLeftx;
													}
											}
									}

									rightArea.width = rightArea.x2 - rightArea.x1;

									mAreas.Add(leftArea);
									mAreas.Add(rightArea);
							}
					}
					mLargestArea = null;

					if (mAreas.Count > 0) // if there is no areas then the whole line is clear, if there is areas, return the largest (it means there is one or more boxes colliding with this line)
					{
							foreach (Area area in mAreas)
							{
									if (mLargestArea == null)
									{
											mLargestArea = area;
									}
									else
									{
											if (area.width > mLargestArea.width)
											{
													mLargestArea = area;
											}
									}
							}

							line.leftBound = mLargestArea.x1;
							line.rightBound = mLargestArea.x2;
					}

					return line;
			}

			internal Area mLargestArea;

			private int getChunk(string text, float maxWidth)
			{
					int length = mTextPaint.BreakText(text, true, maxWidth, null);
					if (length <= 0) // if its 0 or less, return it, can't fit any chars on this line
					{
						return length;
					}
					else if (length >= text.Length) // we can fit the whole string in
					{
						return length;
					}
					else if (text[length - 1] == ' ') // if break char is a space  -- return
					{
						return length;
					}
					else
					{
							if (text.Length > length) // or if the following char is a space then return this length - it is fine
							{
								if (text[length] == ' ')
								{
									return length + 1;
								}
							}
					}

					// otherwise, count back until we hit a space and return that as the break length
					int tempLength = length - 1;
					while (text[tempLength] != ' ')
					{

							//char test = text.charAt(tempLength);
							tempLength--;
							if (tempLength <= 0) // if we count all the way back to 0 then this line cannot be broken, just return the original break length
							{
								return length;
							}
					}

					//char test = text.charAt(tempLength);
					return tempLength + 1; // return the nicer break length which doesn't split a word up

			}

			protected override void OnDraw(Canvas canvas)
			{

					Log.Debug("flowText", "onDraw");

                    base.OnDraw (canvas);

					mViewWidth = this.Width;
					int lowestYCoord = 0;
					boxes.Clear();

					int childCount = this.ChildCount;
					for (int i = 0; i < childCount; i++)
					{
							View child = GetChildAt(i);
							if (child.Visibility != ViewStates.Gone)
							{
									Box box = new Box(this);
									box.topLeftx = (int) child.Left;
									box.topLefty = (int) child.Top;
									box.bottomRightx = box.topLeftx + child.Width;
									box.bottomRighty = box.topLefty + child.Height;
									boxes.Add(box);
									if (box.bottomRighty > lowestYCoord)
									{
										lowestYCoord = box.bottomRighty;
									}
							}
					}

                    //var blocks = mText.ToString().Split ("\n", true);
			    var blocks = mText.ToString ().Split (new[] {"\n"}, StringSplitOptions.RemoveEmptyEntries);

					var charOffsetStart = 0; // tells us where we are in the original string
					var charOffsetEnd = 0; // tells us where we are in the original string
					var lineIndex = 0;
					float xOffset = 0; // left margin off a given line
					float maxWidth = mViewWidth; // how far to the right it can strectch
					float yOffset = 0;
					string thisLineStr;
					int chunkSize;
					int lineHeight = LineHeight;

					List<HtmlObject> lineObjects = new List<FlowTextViewSharp.HtmlObject>();
					object[] spans = new object[0];

					HtmlObject htmlLine; // = new HtmlObject(); // reuse for single plain lines

					mLinks.Clear();

					for (int block_no = 0; block_no <= blocks.Length - 1; block_no++)
					{

							string thisBlock = blocks[block_no];
							if (thisBlock.Length <= 0)
							{
									lineIndex++; //is a line break
									charOffsetEnd += 2;
									charOffsetStart = charOffsetEnd;
							}
							else
							{

									while (thisBlock.Length > 0)
									{
											lineIndex++;
											yOffset = lineIndex * lineHeight;
											Line thisLine = getLine(yOffset, lineHeight);
											xOffset = thisLine.leftBound;
											maxWidth = thisLine.rightBound - thisLine.leftBound;
											float actualWidth = 0;


											do
											{
													Log.Debug("tv", "maxWidth: " + maxWidth);
													chunkSize = getChunk(thisBlock, maxWidth);
													int thisCharOffset = charOffsetEnd + chunkSize;

													if (chunkSize > 1)
													{
															thisLineStr = thisBlock.Substring(0, chunkSize);
													}
													else
													{
															thisLineStr = "";
													}

													lineObjects.Clear();

													if (mIsHtml)
													{
															spans = ((ISpanned) mText).GetSpans(charOffsetStart, thisCharOffset, Class.FromType (typeof(object)));
															if (spans.Length > 0)
															{
																	actualWidth = parseSpans(lineObjects, spans, charOffsetStart, thisCharOffset, xOffset);
															}
															else
															{
																	actualWidth = maxWidth; // if no spans then the actual width will be <= maxwidth anyway
															}
													}
													else
													{
															actualWidth = maxWidth; // if not html then the actual width will be <= maxwidth anyway
													}


													Log.Debug("tv", "actualWidth: " + actualWidth);

													if (actualWidth > maxWidth)
													{
															maxWidth -= 5; // if we end up looping - start slicing chars off till we get a suitable size
													}

											} while (actualWidth > maxWidth);



											// chunk is ok 
											charOffsetEnd += chunkSize;

											Log.Debug("tv", "charOffsetEnd: " + charOffsetEnd);

											if (lineObjects.Count <= 0) // no funky objects found, add the whole chunk as one object
											{
													htmlLine = new HtmlObject(this, thisLineStr, 0, 0, xOffset, mTextPaint);
													lineObjects.Add(htmlLine);
											}

											foreach (HtmlObject thisHtmlObject in lineObjects)
											{

													if (thisHtmlObject is HtmlLink)
													{
															HtmlLink thisLink = (HtmlLink) thisHtmlObject;
															float thisLinkWidth = thisLink.paint.MeasureText(thisHtmlObject.content);
															addLink(thisLink, yOffset, thisLinkWidth, lineHeight);
													}

													paintObject(canvas, thisHtmlObject.content, thisHtmlObject.xOffset, yOffset, thisHtmlObject.paint);

													if (thisHtmlObject.recycle)
													{
															recyclePaint(thisHtmlObject.paint);
													}
											}


											if (chunkSize >= 1)
											{
												thisBlock = thisBlock.Substring(chunkSize, thisBlock.Length - chunkSize);
											}

											charOffsetStart = charOffsetEnd;
									}
							}
					}

					yOffset += (lineHeight / 2);

					View child1 = GetChildAt(ChildCount - 1);
                    if (child1.Tag != null)
					{
                        if (child1.Tag.ToString ().Equals ("hideable", StringComparison.CurrentCultureIgnoreCase))
							{
									if (yOffset > pageHeight)
									{
											if (yOffset < boxes[boxes.Count - 1].topLefty - LineHeight)
											{
                                                child1.Visibility = ViewStates.Gone;
													//lowestYCoord = (int) yOffset;
											}
											else
											{
													//lowestYCoord = boxes.get(boxes.size()-1).bottomRighty + getLineHeight();
                                                child1.Visibility = ViewStates.Visible;
											}
									}
									else
									{
                                        child1.Visibility = ViewStates.Gone;
											//lowestYCoord = (int) yOffset;
									}
							}
					}


					mDesiredHeight = Math.Max(lowestYCoord, (int) yOffset);
					if (needsMeasure)
					{
							needsMeasure = false;
							RequestLayout();
					}
			}


			protected override void OnConfigurationChanged(Configuration newConfig)
			{
                base.OnConfigurationChanged (newConfig);
					Invalidate();
			}

			public override void Invalidate()
			{
					needsMeasure = true;
                    base.Invalidate ();
			}

			internal bool needsMeasure = true;

			private void paintObject(Canvas canvas, string thisLineStr, float xOffset, float yOffset, Paint paint)
			{
					canvas.DrawText(thisLineStr, xOffset, yOffset, paint);
			}

			private class Box
			{
				private readonly FlowTextViewSharp outerInstance;

				public Box(FlowTextViewSharp outerInstance)
				{
					this.outerInstance = outerInstance;
				}

					public int topLeftx;
					public int topLefty;
					public int bottomRightx;
					public int bottomRighty;
			}

			private class Line
			{
				private readonly FlowTextViewSharp outerInstance;

				public Line(FlowTextViewSharp outerInstance)
				{
					this.outerInstance = outerInstance;
				}

					public float leftBound;
					public float rightBound;
			}

			private List<Box> boxes = new List<FlowTextViewSharp.Box>();

			private static readonly BoringLayout.Metrics UNKNOWN_BORING = new BoringLayout.Metrics();




			protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
			{

					Log.Debug("flowText", "onMeasure");
					base.OnMeasure(widthMeasureSpec, heightMeasureSpec);

					MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
                    MeasureSpecMode heightMode = MeasureSpec.GetMode (heightMeasureSpec);
                    int widthSize = MeasureSpec.GetSize (widthMeasureSpec);
                    int heightSize = MeasureSpec.GetSize (heightMeasureSpec);

					int width = 0;
					int height = 0;

					if (widthMode == MeasureSpecMode.Exactly)
					{
							// Parent has told us how big to be. So be it.
							width = widthSize;
					}
					else
					{
							width = Width;
					}

                    if (heightMode == MeasureSpecMode.Exactly)
					{
							// Parent has told us how big to be. So be it.
							height = heightSize;
					}
					else
					{
							height = mDesiredHeight;
					}

					SetMeasuredDimension(width, height + LineHeight);

					//setMeasuredDimension(800, 1400);
			}



			public virtual int LineHeight
			{
				get
				{
						return (int) Math.Round(mTextPaint.GetFontMetricsInt(null) * mSpacingMult + mSpacingAdd);
				}
			}

			private IEnumerable<char> mText = "";

			private bool mIsHtml = false;
			//private URLSpan[] urls;

			internal class HtmlObject
			{
				private readonly FlowTextViewSharp outerInstance;


					public HtmlObject(FlowTextViewSharp outerInstance, string content, int start, int end, float xOffset, TextPaint paint) : base()
					{
						this.outerInstance = outerInstance;
							this.content = content;
							this.start = start;
							this.end = end;
							this.xOffset = xOffset;
							this.paint = paint;
					}
					public string content;
					public int start;
					public int end;
					public float xOffset;
					public TextPaint paint;
					public bool recycle = false;
			}

			internal class HtmlLink : HtmlObject
			{
				private readonly FlowTextViewSharp outerInstance;

					public HtmlLink(FlowTextViewSharp outerInstance, string content, int start, int end, float xOffset, TextPaint paint, string url) : base(outerInstance, content, start, end, xOffset, paint)
					{
						this.outerInstance = outerInstance;
							this.url = url;
					}
					public float width;
					public float height;
					public float yOffset;
					public string url;
			}


			private bool[] charFlags;
			internal int charFlagSize = 0;
			internal int charFlagIndex = 0;
			internal int spanStart = 0;
			internal int spanEnd = 0;
			internal int charCounter;
			internal float objPixelwidth;

			internal Dictionary<int?, HtmlObject> sorterMap = new Dictionary<int?, FlowTextViewSharp.HtmlObject>();
			private float parseSpans(List<HtmlObject> objects, object[] spans, int lineStart, int lineEnd, float baseXOffset)
			{

					sorterMap.Clear();

					charFlagSize = lineEnd - lineStart;
					charFlags = new bool[charFlagSize];

					foreach (var span in spans)
					{
							spanStart = mSpannable .GetSpanStart((Object) span);
							spanEnd = mSpannable.GetSpanEnd((Object) span);

							if (spanStart < lineStart)
							{
								spanStart = lineStart;
							}
							if (spanEnd > lineEnd)
							{
								spanEnd = lineEnd;
							}

							for (charCounter = spanStart; charCounter < spanEnd; charCounter++) // mark these characters as rendered
							{
									charFlagIndex = charCounter - lineStart;
									charFlags[charFlagIndex] = true;
							}

							tempString = extractText(spanStart, spanEnd);
							sorterMap[spanStart] = parseSpan(span, tempString, spanStart, spanEnd);
							//objects.add();                        
					}

					charCounter = 0;

					while (!isArrayFull(charFlags))
					{
							while (true)
							{

									if (charCounter >= charFlagSize)
									{
										break;
									}


									if (charFlags[charCounter] == true)
									{
											charCounter++;
											continue;
									}

									temp1 = charCounter;
									while (true)
									{
											if (charCounter > charFlagSize)
											{
												break;
											}

											if (charCounter < charFlagSize)
											{
													if (charFlags[charCounter] == false)
													{

															charFlags[charCounter] = true; // mark as filled
															charCounter++;
															continue;

													}
											}
											temp2 = charCounter;
											spanStart = lineStart + temp1;
											spanEnd = lineStart + temp2;
											tempString = extractText(spanStart, spanEnd);
											sorterMap[spanStart] = parseSpan(null, tempString, spanStart, spanEnd);
											break;

									}
							}
					}

					sorterKeys = new[] {sorterMap.Keys.ToArray ()};
					Arrays.Sort((Object[]) sorterKeys);

					float thisXoffset = baseXOffset;

					for (charCounter = 0; charCounter < sorterKeys.Length; charCounter++)
					{
							HtmlObject thisObj = sorterMap[(int?) sorterKeys[charCounter]];
							thisObj.xOffset = thisXoffset;
							tempFloat = thisObj.paint.MeasureText(thisObj.content);
							thisXoffset += tempFloat;
							objects.Add(thisObj);
					}

					return (thisXoffset - baseXOffset);
			}

			internal float tempFloat;
			internal object[] sorterKeys;
			internal int[] sortedKeys;
			internal string tempString;
			internal int temp1;
			internal int temp2;

			internal int arrayIndex = 0;
			private bool isArrayFull(bool[] array)
			{
					for (arrayIndex = 0; arrayIndex < array.Length; arrayIndex++)
					{
							if (array[arrayIndex] == false)
							{
								return false;
							}
					}
					return true;
			}

			private HtmlObject parseSpan(object span, string content, int start, int end)
			{

					if (span is URLSpan)
					{
							return getHtmlLink((URLSpan) span, content, start, end, 0);
					}
					else if (span is StyleSpan)
					{
							return getStyledObject((StyleSpan) span, content, start, end, 0);
					}
					else
					{
							return getHtmlObject(content, start, end, 0);
					}
			}

			private string extractText(int start, int end)
			{
					if (start < 0)
					{
						start = 0;
					}
					if (end > mTextLength - 1)
					{
						end = mTextLength - 1;
					}
					return mSpannable.SubSequence(start, end).ToString();
			}


			private List<TextPaint> mPaintHeap = new List<TextPaint>();

			private TextPaint PaintFromHeap
			{
				get
				{
						if (mPaintHeap.Count > 0) {
						    var res = mPaintHeap.First ();
                            mPaintHeap.Remove (res);
						    return res;
						}
						else
						{
								return new TextPaint(PaintFlags.AntiAlias);
						}
				}
			}

			private void recyclePaint(TextPaint paint)
			{
					mPaintHeap.Add(paint);
			}

			private HtmlObject getStyledObject(StyleSpan span, string content, int start, int end, float thisXOffset)
			{
					TextPaint paint = PaintFromHeap;
					paint.SetTypeface(Typeface.DefaultFromStyle(span.Style));
					paint.TextSize = mTextsize;
					paint.Color = mColor;

					span.UpdateDrawState(paint);
					span.UpdateMeasureState(paint);
					HtmlObject obj = new HtmlObject(this, content, start, end, thisXOffset, paint);
					obj.recycle = true;
					return obj;
			}

			private HtmlObject getHtmlObject(string content, int start, int end, float thisXOffset)
			{
					HtmlObject obj = new HtmlObject(this, content, start, end, thisXOffset, mTextPaint);
					return obj;
			}

			private List<HtmlLink> mLinks = new List<FlowTextViewSharp.HtmlLink>();

			private HtmlLink getHtmlLink(URLSpan span, string content, int start, int end, float thisXOffset)
			{
					HtmlLink obj = new HtmlLink(this, content, start, end, thisXOffset, mLinkPaint, span.URL);
					mLinks.Add(obj);
					return obj;
			}

			private void addLink(HtmlLink thisLink, float yOffset, float width, float height)
			{
					thisLink.yOffset = yOffset - 20;
					thisLink.width = width;
					thisLink.height = height + 20;
					mLinks.Add(thisLink);

			}

            private SpannableString mSpannable;


			internal int mTextLength = 0;
			public virtual IEnumerable<char> Text
			{
				set
				{
						mText = value;
                        if (value is SpannableString)
						{
								mIsHtml = true;
                                mSpannable = (SpannableString)value;
								object[] urls = mSpannable.GetSpans(0, mSpannable.Length(), Class.FromType (typeof(object)));
						}
						else
						{
								mIsHtml = false;
						}

				    mTextLength = mText.Count ();
    
						this.Invalidate();
				}
			}


			private List<BitmapSpec> bitmaps = new List<FlowTextViewSharp.BitmapSpec>();

			public virtual BitmapSpec addImage(Bitmap bitmap, int xOffset, int yOffset, int padding)
			{
					BitmapSpec spec = new BitmapSpec(this, bitmap, xOffset, yOffset, padding);
					bitmaps.Add(spec);
					return spec;
			}

			public virtual List<BitmapSpec> Bitmaps
			{
				get
				{
						return bitmaps;
				}
				set
				{
						this.bitmaps = value;
				}
			}


			public class BitmapSpec
			{
				private readonly FlowTextViewSharp outerInstance;


					public BitmapSpec(FlowTextViewSharp outerInstance, Bitmap bitmap, int xOffset, int yOffset, int mPadding) : base()
					{
						this.outerInstance = outerInstance;
							this.bitmap = bitmap;
							this.xOffset = xOffset;
							this.yOffset = yOffset;
							this.mPadding = mPadding;
					}
					public Bitmap bitmap;
					public int xOffset;
					public int yOffset;
					public int mPadding = 10;
			}

			public virtual int PageHeight
			{
				set
				{
						this.pageHeight = value;
				}
			}


	}


}