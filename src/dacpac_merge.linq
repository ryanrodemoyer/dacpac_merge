<Query Kind="Program">
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.Data.Tools.Schema.Sql.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.Data.Tools.Utilities.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.SqlServer.Dac.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.SqlServer.Dac.Extensions.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.SqlServer.TransactSql.ScriptDom.dll</Reference>
  <Reference>&lt;ProgramFilesX64&gt;\Microsoft SQL Server\150\DAC\bin\Microsoft.SqlServer.Types.dll</Reference>
  <NuGetReference>System.IO.Packaging</NuGetReference>
  <Namespace>Microsoft.SqlServer.Dac</Namespace>
  <Namespace>Microsoft.SqlServer.Dac.Model</Namespace>
  <Namespace>Microsoft.SqlServer.TransactSql.ScriptDom</Namespace>
  <Namespace>System.IO.Packaging</Namespace>
</Query>

void Main()
{
	void dacfx_explore()
	{
		TSqlModel getModel(string source)
		{
			bool exists = File.Exists(source);
			if (exists)
			{
				return new TSqlModel(source);
			}

			throw new ArgumentException("File does not exist.");
		}

		string dacpac = @"C:\RyanData\ELCGIT2017_1.1\DatabaseProjects\ELC\Cadence\bin\Release\Cadence.dacpac";
		var model = getModel(dacpac);


		var objs = model.GetObjects(DacQueryScopes.UserDefined);

		foreach (var obj in objs.Take(10))
		{
			TSqlScript ast;
			if (obj.TryGetAst(out ast))
			{
				var src = obj.GetScript();
				src.Dump($"{obj.Name} - {obj.ObjectType.Name}");
				//ast.Dump();
			}
			else
			{
				"ast unavailable".Dump();
			}
		}


		model.CopyModelOptions().Dump();

		objs.Dump();
	}
	
	void dacpac_merge()
	{
		string root = Path.GetDirectoryName(Util.CurrentQueryPath);
		
		string target = Path.Combine(root, @"..\_output\merged.dacpac");
		string[] others = {
			Path.Combine(root, @"..\_output\Database1.dacpac"),
			Path.Combine(root, @"..\_output\Database2.dacpac"),
		};
		
		var merge = new DacpacMerge(target, others);
		merge.Merge();
	}

	dacpac_merge();
}

// source: https://the.agilesql.club/2019/03/how-can-we-merge-multiple-dacpacs-into-one/
class DacpacMerge
{

	private string[] _sources;
	private TSqlModel _first;
	private string _targetPath;
	private TSqlModel _target;

	public DacpacMerge(string target, params string[] sources)
	{
		_sources = sources;
		_first = new TSqlModel(sources.First<string>());
		var options = _first.CopyModelOptions();

		_target = new TSqlModel(_first.Version, options);
		_targetPath = target;
	}

	public void Merge()
	{
		string pre = string.Empty;
		string post = string.Empty;

		foreach (string source in _sources)
		{
			Console.WriteLine($"Processing {source}");
			
			var model = getModel(source);
			foreach (var obj in model.GetObjects(DacQueryScopes.UserDefined))
			{
				TSqlScript ast;
				if (obj.TryGetAst(out ast))
				{
					var name = obj.Name.ToString();
					var info = obj.GetSourceInformation();
					if (info != null && !string.IsNullOrWhiteSpace(info.SourceName))
					{
						name = info.SourceName;
					}

					if (!string.IsNullOrWhiteSpace(name) && !name.EndsWith(".xsd"))
					{
						_target.AddOrUpdateObjects(ast, name, new TSqlObjectOptions());    //WARNING throwing away ansi nulls and quoted identifiers!
					}
				}
			}

			using (var package = DacPackage.Load(source))
			{
				if (!(package.PreDeploymentScript is null))
				{
					pre += new StreamReader(package.PreDeploymentScript).ReadToEnd();
				}
				if (!(package.PostDeploymentScript is null))
				{
					post += new StreamReader(package.PostDeploymentScript).ReadToEnd();
				}
			}
		}

		WriteFinalDacpac(_target, pre, post);
	}

	private void WriteFinalDacpac(TSqlModel model, string preScript, string postScript)
	{
		Console.WriteLine($"WriteFinalDacpac, _targetPath={_targetPath}");

		var metadata = new PackageMetadata();
		metadata.Name = "dacpac";

		DacPackageExtensions.BuildPackage(_targetPath, model, metadata);
		AddScripts(preScript, postScript, _targetPath);
	}

	TSqlModel getModel(string source)
	{
		if (source == _sources.FirstOrDefault<string>())
		{
			return _first;
		}

		return new TSqlModel(source);
	}

	private void AddScripts(string pre, string post, string dacpacPath)
	{
		Console.WriteLine($"AddScripts, dacpacPath={dacpacPath}");
		
		using (Package package = Package.Open(dacpacPath, FileMode.Open, FileAccess.ReadWrite))
		{
			if (!string.IsNullOrEmpty(pre))
			{
				PackagePart part = package.CreatePart(new Uri("/predeploy.sql", UriKind.Relative), "text/plain");

				using (var stream = part.GetStream())
				{
					stream.Write(Encoding.UTF8.GetBytes(pre), 0, pre.Length);
				}
			}

			if (!string.IsNullOrEmpty(post))
			{
				PackagePart part = package.CreatePart(new Uri("/postdeploy.sql", UriKind.Relative), "text/plain");

				using (var stream = part.GetStream())
				{
					stream.Write(Encoding.UTF8.GetBytes(post), 0, post.Length);
				}
			}
			
			package.Close();
		}
	}
}