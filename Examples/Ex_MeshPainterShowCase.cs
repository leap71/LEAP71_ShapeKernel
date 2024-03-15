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
        class MeshPainterShowCase
        {
            public static void Task()
            {
                //Step 1: Generate a three example meshes (e.g. from voxelfields)
                float fRadius               = 50f;
                BaseSphere oSphere1         = new BaseSphere(new LocalFrame(new Vector3(-120, 0, 0)), fRadius);
                BaseSphere oSphere2         = new BaseSphere(new LocalFrame(new Vector3(   0, 0, 0)), fRadius);
                BaseSphere oSphere3         = new BaseSphere(new LocalFrame(new Vector3(+120, 0, 0)), fRadius);
                Mesh msh1                   = new Mesh(oSphere1.voxConstruct());
                Mesh msh2                   = new Mesh(oSphere2.voxConstruct());
                Mesh msh3                   = new Mesh(oSphere3.voxConstruct());


                //Step 2: Show local overhang distribution on first mesh
                float fMinValue             =  0;   // deg
                float fMaxValue             = 90;   // deg
                bool bShowOnlyDownFacing    = false;
                uint nColorSteps            = 50;
                IColorScale xScale1         = new ColorScale3D(new RainboxSpectrum(), fMinValue, fMaxValue);
                MeshPainter.PreviewOverhangAngle(msh1, xScale1, bShowOnlyDownFacing, nColorSteps);


                //Step 3: Show critical overhang areas on second mesh
                float fCritAngle            = 30;   // deg
                float fTransitionSmooth     = 5;
                bShowOnlyDownFacing         = true;
                IColorScale xScale2         = new CustomColorScale2D(Cp.clrGreen, Cp.clrRed, fMinValue, fMaxValue, fCritAngle, fTransitionSmooth);
                MeshPainter.PreviewOverhangAngle(msh2, xScale2, bShowOnlyDownFacing, nColorSteps);

                
                //Step 4: Show custom property on third mesh
                fMinValue                   = 0;    // mm
                fMaxValue                   = 10;   // mm
                IColorScale xScale3         = new LinearColorScale2D(Cp.clrCrystal, Cp.clrPitaya, fMinValue, fMaxValue);
                MeshPainter.PreviewCustomProperty(msh3, xScale3, fGetExampleProperty, nColorSteps);
            }

            /// <summary>
            /// Example function to calculate a custom property of a given triangle.
            /// In this case, the x-coordinate of the triangle is evaluated in a modulus function.
            /// The output value of this function (with respect to the min and max value of the scale) will determine the color
            /// in which the triangle will be displayed.
            /// A, B, C are the input mesh triangle vertices.
            /// </summary>
            public static float fGetExampleProperty(Vector3 vecA, Vector3 vecB, Vector3 vecC)
            {
                Vector3 vecCentre   = (vecA + vecB + vecC) / 3f;
                float fValue        = (MathF.Abs(vecCentre.X) % 10);
                return fValue;
            }
        }
    }
}