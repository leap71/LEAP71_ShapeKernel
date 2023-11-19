//
// SPDX-License-Identifier: CC0-1.0
//
// This example code file is released to the public under Creative Commons CC0.
// See https://creativecommons.org/publicdomain/zero/1.0/legalcode
//
// To the extent possible under law, LEAP 71 has waived all copyright and
// related or neighboring rights to this LEAP 71 ShapeKernel Example Code.
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


namespace Leap71
{
    using System.Numerics;
    using ShapeKernel;

    namespace ShapeKernelExamples
    {
        class LatticePipeShowCase
        {
            public static void Task()
            {
                try
                {
                    {
                        //basic
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(-50, 0, 0));
                        LatticePipe oShape      = new LatticePipe(oLocalFrame, 60f, 10);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrYellow);
                    }

                    {
                        //modulated 0
                        LocalFrame oLocalFrame  = new LocalFrame(new Vector3(50, -50, 0));
                        LatticePipe oShape      = new LatticePipe(oLocalFrame, 60);
                        oShape.SetRadius(new LineModulation(fGetLineModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrFrozen);
                    }

                    {
                        //modulated + spined
                        ISpline oSpine          = new ExampleSpline();
                        Frames aFrames          = new Frames(oSpine.aGetPoints(), Vector3.UnitY);
                        LatticePipe oShape      = new LatticePipe(aFrames);
                        oShape.SetRadius(new LineModulation(fGetLineModulation1));
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrRacingGreen);
                    }

                    {
                        //spined
                        ISpline oSpine          = new ExampleSpline();
                        List<Vector3> aPoints   = SplineOperations.aTranslateList(oSpine.aGetPoints(), 50 * Vector3.UnitX);
                        Frames aFrames          = new Frames(aPoints, Vector3.UnitY);
                        LatticePipe oShape      = new LatticePipe(aFrames, 5f);
                        Voxels oVoxels          = oShape.voxConstruct();
                        Sh.PreviewVoxels(oVoxels, Cp.clrOrchid);
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