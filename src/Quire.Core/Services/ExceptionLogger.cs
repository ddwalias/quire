#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Quire.Mirror;

namespace Quire.Services;

public partial class ExceptionLogger
{
    public string LogException(Exception exception)
    {
        return LogExceptions(new Exception[1] { exception });
    }

    public virtual string[] GetExceptionData(IEnumerable<Exception> exceptions)
    {
        return new string[5]
        {
            GetMiscInfo(),
            GetExceptionInfo(exceptions),
            GetApplicationStartTime(),
            GetEnvironmentVariablesAndCommandLineArgumentsData(),
            GetListOfLoadedModulesData()
        };
    }

    public string GetExceptionInfo(IEnumerable<Exception> exceptions)
    {
        StringBuilder exceptionData = new StringBuilder();
        exceptions.Each((Exception exception) =>
        {
            exceptionData.AppendLine("Exception data:");
            foreach (DictionaryEntry datum in exception.Data)
            {
                exceptionData.AppendFormat("{0}: {1}", datum.Key, datum.Value).AppendLine();
            }
            FileFormatException ex = (exception as FileFormatException) ?? (exception.InnerException as FileFormatException);
            if (ex != null)
            {
                exceptionData.AppendLine(string.Format("FileFormatException when processing file {0}", ex.SourceUri));
            }
            XmlSchemaValidationException ex2 = (exception as XmlSchemaValidationException) ?? (exception.InnerException as XmlSchemaValidationException);
            if (ex2 != null)
            {
                exceptionData.AppendLine(string.Format("Schema validation exception: {0}", ex2.Message));
                if (ex2.SourceObject != null)
                {
                    exceptionData.AppendLine(string.Format("SourceObject: {0}", ex2.SourceObject));
                    if (ex2.SourceObject is XmlElement xmlElement)
                    {
                        if (xmlElement.OwnerDocument != null && xmlElement.OwnerDocument.DocumentElement != null)
                        {
                            exceptionData.AppendLine(string.Format("OwnerDocument DocumentElement Name {0}", xmlElement.OwnerDocument.DocumentElement.Name));
                        }
                        GetElementData(xmlElement, exceptionData, "SourceObject");
                        GetElementData(xmlElement.ParentNode, exceptionData, "SourceObject Parent");
                    }
                }
            }
            exceptionData.AppendLine(exception.ToString());
            exceptionData.AppendLine().AppendLine();
        });
        return exceptionData.ToString();
    }

    private static void GetElementData(XmlNode xmlNode, StringBuilder exceptionData, string elementType)
    {
        if (xmlNode != null)
        {
            goto HasNode;
        }
        return;

    HasNode:
        exceptionData.AppendLine(string.Format("{0} Name: {1}", elementType, xmlNode.Name));
        string outerXml = xmlNode.OuterXml;
        if (string.IsNullOrEmpty(outerXml))
        {
            goto Done;
        }
        exceptionData.AppendLine(string.Format("{0} {1} OuterXml: {2}", elementType, xmlNode.Name, outerXml));

    Done:
        return;
    }

    public string LogExceptions(IEnumerable<Exception> exceptions)
    {
        string message = null;
        bool condition = true;
        try
        {
            if (exceptions.Any((Exception e) => e == null))
            {
                message = "Exception is passed as null";
                condition = false;
            }
            TextWriter textWriter = CreateApplicationLog("Exception");
            try
            {
                GetExceptionData(exceptions).Each((string data) =>
                {
                    WriteToLog(data, textWriter);
                });
            }
            finally
            {
                if (textWriter != null)
                {
                    ((IDisposable)textWriter).Dispose();
                }
            }
        }
        catch (Exception ex)
        {
            message = ex.Message;
            condition = false;
        }
        DebugTools.DevAssert(condition, message);
        return _applicationLogFilePath;
    }

    [MirrorVerified("PtcServicesExceptionLoggerMechanicalDifferentialTests")]
    public ExceptionLogger() { }
}
