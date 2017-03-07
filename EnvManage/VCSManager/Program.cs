using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System.IO;
using SharpSvn;
using log4net;
using log4net.Config;

namespace VCSManager
{
    class Program
    {
        static SvnClient svnClient;
        static int directoryCnt = 0;
        static ILog log;

        static void Main(string[] args)
        {
            // XmlConfigurator.Configure(new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml")));
            XmlConfigurator.Configure();
            log = LogManager.GetLogger(typeof(Program));

            log.Info("VCSManager started");
    

            svnClient = new SvnClient();

            List<String> driveNameList = new List<string> { "D", "E", "F", "G" };
            foreach(String driveName in driveNameList)
            {
                DriveInfo driveInfo = new DriveInfo(driveName);
                processDir(driveInfo.RootDirectory, 3);
            }
           log.Info("directory processed: "+ directoryCnt);
            // Console.ReadLine();
        }

        static void processDir(DirectoryInfo dirInfo, int depth)
        {
            directoryCnt++;

            if (Repository.IsValid(dirInfo.FullName))
            {
                log.Info("pulling git repo: " + dirInfo.FullName);
                var gitRepo = new Repository(dirInfo.FullName);
                PullOptions options = new PullOptions();
                options.FetchOptions = new FetchOptions();
                options.FetchOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new DefaultCredentials());
                try
                {
                    gitRepo.Network.Pull(new Signature("Henry.T", "tang.hy@live.com", DateTime.Now), options);
                }
                catch (LibGit2SharpException lge)
                {
                    if (lge.Message.StartsWith("Unsupported URL protocol"))
                    {
                        log.Warn("failed to pull git repo: " + dirInfo.FullName);
                    }else if (lge.Message.StartsWith("Too many redirects or authentication replays"))
                    {
                        // TODO ..
                        log.Warn("failed to pull git repo: " + dirInfo.FullName);
                    }
                    else
                    {
                        log.Error("UNKNOWN ERROR failed to pull git repo: " + dirInfo.FullName);
                    }
                }

                log.Info("pushing git repo to origin");
                PushOptions pushOptions = new PushOptions();
                pushOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new DefaultCredentials());
                try
                {
                    var branch = gitRepo.Branches["master"];
                    if (branch == null)
                    {
                        log.Warn("only support git repo with master branch");
                    }
                    else
                    {
                        gitRepo.Network.Push(gitRepo.Branches["master"], pushOptions);
                    }
                }
                catch (LibGit2SharpException lge)
                {
                    if (lge.Message.StartsWith("Unsupported URL protocol"))
                    {
                        log.Warn("failed to push git repo: Unsupported URL protocol");
                    }
                    else if (lge.Message.StartsWith("Too many redirects or authentication replays"))
                    {
                        // TODO ..
                        log.Warn("failed to pull git repo: Too many redirects or authentication replays");
                    }
                    else
                    {
                        log.Error("UNKNOWN ERROR failed to push git repo: " + dirInfo.FullName);
                    }
                } 
            }
            else if (svnClient.GetUriFromWorkingCopy(dirInfo.FullName) != null)
            {
                log.Info("updateing svn working copy: " + dirInfo.FullName);
                try {
                    svnClient.Update(dirInfo.FullName);
                }
                catch(Exception e)
                {
                    if (e.Message.StartsWith("Unable to connect to a repository at URL"))
                    {
                       log.Warn("SVN update failed: " + e.Message);
                    } else if (e.Message.StartsWith("Previous operation has not finished; run 'cleanup' if it was interrupted"))
                    {
                        log.Warn("SVN update failed: " + e.Message);
                    }
                    else if (e.Message.StartsWith("Working copy") && e.Message.EndsWith(" locked."))
                    {
                        //Working copy 'E:\Archive\AnyShoot\AnyShootDemo' locked.
                        log.Warn("SVN update failed: " + e.Message);
                    }
                    else
                    {
                        log.Error("SVN update failed UNKNOWN ERROR! : " + e.Message);
                    }
                }
                
            }
            else if (depth > 0)
            {
                try
                {
                    foreach (var subDirInfo in dirInfo.GetDirectories())
                    {
                        processDir(subDirInfo, depth - 1);
                    }
                }
                catch(UnauthorizedAccessException uaex)
                {
                    
                }
            }
        }
    }
}
