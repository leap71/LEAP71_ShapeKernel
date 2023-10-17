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


using System.Numerics;


namespace Leap71
{
    namespace ShapeKernel
    {
        public class ExampleSpline : ISpline
        {
            protected ControlPointSpline m_oBSpline;
            public ExampleSpline()
            {
                List<Vector3> aControlPoints = new List<Vector3>() {
                    new Vector3(0, 0, 0),
                    new Vector3(0, 40, 0),
                    new Vector3(0, 50, 20),
                    new Vector3(0, 60, 60),
                };
                m_oBSpline = new ControlPointSpline(aControlPoints);
            }

            public List<Vector3> aGetPoints(uint nSamples = 500)
            {
                return m_oBSpline.aGetPoints(nSamples);
            }
        }
    }
}