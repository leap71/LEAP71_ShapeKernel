//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this PicoGK Example Code.
//
// THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//


using PicoGK;
using System.Numerics;


namespace Leap71
{
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class BaseBoxShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 0, 0));
                        BaseBox oShape          = new BaseBox(oLocalFrame, 20, 10, 15);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrBlue);
                    }

                    {
                        //modulated
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, 0, 0));
                        BaseBox oShape          = new BaseBox(oLocalFrame, 20);
                        oShape.SetWidth(new LineModulation(fGetLineModulation2));
                        oShape.SetDepth(new LineModulation(fGetLineModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrGreen);
                    }
                    {
                        //modulated + spined
                        ISpline oSpine          = new ExampleSpline();
                        Frames aFrames          = new Frames(oSpine.aGetPoints(), Vector3.UnitY);
                        BaseBox oShape          = new BaseBox(aFrames);
                        oShape.SetWidth(new LineModulation(fGetLineModulation2));
                        oShape.SetDepth(new LineModulation(fGetLineModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrYellow);
                    }
                }
                catch (Exception e)
                {
                    Library.Log($"Failed run example: \n{e.Message}"); ;
                }
            }

            //functions for generic line modulations
            public static float fGetLineModulation1(float fLengthRatio)
            {
                float fWidth = 10f - 3f * MathF.Cos(8f * fLengthRatio);
                return fWidth;
            }

            public static float fGetLineModulation2(float fLengthRatio)
            {
                float fDepth = 8f - 1f * MathF.Cos(40f * fLengthRatio);
                return fDepth;
            }
        }
    }
}