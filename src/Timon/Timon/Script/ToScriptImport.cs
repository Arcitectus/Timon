using Bib3;
using SmartNet;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Timon.Script.Impl
{
	static public class ToScriptImport
	{
		static readonly Type[] AssemblyAndNamespaceAdditionType = new Type[]
		{
			typeof(List<>),
		};

		static readonly Type[] NamespaceStaticAdditionType = new[]
		{
			typeof(int),
			typeof(ISmartSession),
			typeof(IHostToScript),
			typeof(BotSharp.ToScript.IHostToScript),
			typeof(ToScriptExtension),
			typeof(Bib3.Extension),
			typeof(Enumerable),
			typeof(System.Text.RegularExpressions.Regex),
		};

		static IEnumerable<Type> AssemblyAdditionType => new[]
		{
			AssemblyAndNamespaceAdditionType,
			NamespaceStaticAdditionType,
		}.ConcatNullable();

		static public IEnumerable<string> AssemblyName =>
			AssemblyAdditionType?.Select(t => t.Assembly.GetName()?.Name)?.Distinct();

		static public IEnumerable<Microsoft.CodeAnalysis.MetadataReference> ImportAssembly =>
			AssemblyName?.Select(AssemblyName => GetAssemblyReference(AssemblyName));

		static public IEnumerable<string> ImportNamespace =>
			new[]
			{
				AssemblyAndNamespaceAdditionType?.Select(t => t.Namespace),
				NamespaceStaticAdditionType?.Select(t => t.FullName),
			}.ConcatNullable();

		static readonly Func<string, Stream> CosturaAssemblyResolver = Costura.AssemblyResolverCosturaConstruct();

		static public Microsoft.CodeAnalysis.MetadataReference GetAssemblyReference(string AssemblyName)
		{
			var FromCosturaStream = CosturaAssemblyResolver?.Invoke(AssemblyName);

			if (null != FromCosturaStream)
			{
				return Microsoft.CodeAnalysis.MetadataReference.CreateFromStream(FromCosturaStream);
			}

			return AssemblyReferenceFromLoadedAssembly(AssemblyName);
		}

		static Microsoft.CodeAnalysis.MetadataReference AssemblyReferenceFromLoadedAssembly(string AssemblyName)
		{
			var Assembly = AppDomain.CurrentDomain.GetAssemblies()?.FirstOrDefault(Candidate => Candidate?.GetName()?.Name == AssemblyName);

			if (null == Assembly)
			{
				return null;
			}

			return Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(Assembly.Location);
		}

		static public CultureInfo ScriptDefaultCulture => CultureInfo.InvariantCulture;
	}
}
