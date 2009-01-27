/*
 * Created by SharpDevelop.
 * User: lextm
 * Date: 2008/5/1
 * Time: 18:10
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace Lextm.SharpSnmpLib
{
    /// <summary>
    /// GET response PDU.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pdu")]
    public class GetResponsePdu : ISnmpPdu
    {
        private readonly Integer32 _errorStatus;
        private readonly Integer32 _sequenceNumber;
        private readonly Integer32 _errorIndex;
        private readonly IList<Variable> _variables;
        private readonly Sequence _varbindSection;
        private readonly byte[] _raw;
                
        /// <summary>
        /// Creates a <see cref="GetResponsePdu"/> with all contents.
        /// </summary>
        /// <param name="errorStatus">Error status.</param>
        /// <param name="errorIndex">Error index.</param>
        /// <param name="sequenceNumber">Sequence number.</param>
        /// <param name="variables">Variables.</param>
        public GetResponsePdu(Integer32 sequenceNumber, ErrorCode errorStatus, Integer32 errorIndex, IList<Variable> variables)
        {
            _sequenceNumber = sequenceNumber;
            _errorStatus = new Integer32((int)errorStatus);
            _errorIndex = errorIndex;
            _variables = variables;
            _varbindSection = Variable.Transform(variables);
            _raw = ByteTool.ParseItems(_sequenceNumber, _errorStatus, _errorIndex, _varbindSection);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GetResponsePdu"/> class.
        /// </summary>
        /// <param name="length">The length.</param>
        /// <param name="stream">The stream.</param>
        public GetResponsePdu(int length, Stream stream)
        {
            _sequenceNumber = (Integer32)DataFactory.CreateSnmpData(stream);
            _errorStatus = (Integer32)DataFactory.CreateSnmpData(stream);
            _errorIndex = (Integer32)DataFactory.CreateSnmpData(stream);
            _varbindSection = (Sequence)DataFactory.CreateSnmpData(stream);
            _variables = Variable.Transform(_varbindSection);
            _raw = ByteTool.ParseItems(_sequenceNumber, _errorStatus, _errorIndex, _varbindSection);
            Debug.Assert(length >= _raw.Length, "length not match");
        }

        internal int SequenceNumber
        {
            get
            {
                return _sequenceNumber.ToInt32();
            }
        }
        
        /// <summary>
        /// Error status.
        /// </summary>
        public ErrorCode ErrorStatus
        {
            get { return (ErrorCode)_errorStatus.ToInt32(); }
        }
        
        /// <summary>
        /// Error index.
        /// </summary>
        public int ErrorIndex
        {
            get { return _errorIndex.ToInt32(); }
        }
        
        /// <summary>
        /// Variables.
        /// </summary>
        public IList<Variable> Variables
        {
            get
            {
                return _variables;
            }
        }
        
        /// <summary>
        /// Type code.
        /// </summary>
        public SnmpType TypeCode
        {
            get
            {
                return SnmpType.GetResponsePdu;
            }
        }
        
        /// <summary>
        /// Converts to message body.
        /// </summary>
        /// <param name="version">Protocol version</param>
        /// <param name="community">Community name</param>
        /// <returns></returns>
        public Sequence ToMessageBody(VersionCode version, OctetString community)
        {
            return ByteTool.PackMessage(version, community, this);
        }

        /// <summary>
        /// Appends the bytes to <see cref="Stream"/>.
        /// </summary>
        /// <param name="stream">The stream.</param>
        public void AppendBytesTo(Stream stream)
        {
            ByteTool.AppendBytes(stream, TypeCode, _raw);
        }

        /// <summary>
        /// Returns a <see cref="string"/> that represents this <see cref="GetResponsePdu"/>/
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "GET response PDU: seq: {0}; status: {1}; index: {2}; variable count: {3}",
                _sequenceNumber, 
                _errorStatus, 
                _errorIndex, 
                _variables.Count.ToString(CultureInfo.InvariantCulture));
        }
    }
}
