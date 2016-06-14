using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibGit2Sharp;
using LibGit2Sharp.Handlers;
using System.IO;
using SharpSvn;

namespace VCSManager
{
    class Program
    {
        static SvnClient svnClient;
        static int directoryCnt = 0;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello VCSManager");

            svnClient = new SvnClient();

            List<String> driveNameList = new List<string> { "D", "E", "F", "G" };
            foreach(String driveName in driveNameList)
            {
                DriveInfo driveInfo = new DriveInfo(driveName);
                processDir(driveInfo.RootDirectory, 3);
            }
            Console.WriteLine("directory processed: "+ directoryCnt);
            Console.ReadLine();
        }

        static void processDir(DirectoryInfo dirInfo, int depth)
        {
            directoryCnt++;

            if (Repository.IsValid(dirInfo.FullName))
            {
                Console.WriteLine("pulling git repo: " + dirInfo.FullName);
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
                        Console.WriteLine("failed to pull git repo: " + dirInfo.FullName);
                    }else if (lge.Message.StartsWith("Too many redirects or authentication replays"))
                    {
                        // TODO ..
                        Console.WriteLine("failed to pull git repo: " + dirInfo.FullName);
                    }
                    else
                    {
                        Console.WriteLine("UNKNOWN ERROR failed to pull git repo: " + dirInfo.FullName);
                    }
                }

                Console.WriteLine("pushing git repo to origin");
                PushOptions pushOptions = new PushOptions();
                pushOptions.CredentialsProvider = new CredentialsHandler((url, usernameFromUrl, types) => new DefaultCredentials());
                try
                {
                    var branch = gitRepo.Branches["master"];
                    if (branch == null)
                    {
                        Console.WriteLine("only support git repo with master branch");
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
                        Console.WriteLine("failed to push git repo: Unsupported URL protocol");
                    }
                    else if (lge.Message.StartsWith("Too many redirects or authentication replays"))
                    {
                        // TODO ..
                        Console.WriteLine("failed to pull git repo: Too many redirects or authentication replays");
                    }
                    else
                    {
                        Console.WriteLine("UNKNOWN ERROR failed to push git repo: " + dirInfo.FullName);
                    }
                } 
            }
            else if (svnClient.GetUriFromWorkingCopy(dirInfo.FullName) != null)
            {
                Console.WriteLine("updateing svn working copy: " + dirInfo.FullName);
                try {
                    svnClient.Update(dirInfo.FullName);
                }
                catch(Exception e)
                {
                    if (e.Message.StartsWith("Unable to connect to a repository at URL"))
                    {
                        Console.WriteLine("SVN update failed: " + e.Message);
                    } else if (e.Message.StartsWith("Previous operation has not finished; run 'cleanup' if it was interrupted"))
                    {
                        Console.WriteLine("SVN update failed: " + e.Message);
                    }
                    else
                    {
                        Console.WriteLine("SVN update failed UNKNOWN ERROR! : " + e.Message);
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
