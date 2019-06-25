# Filewatcher-WinForms
The Filewatcher-WinForms application runs on windows by utilizing the windows forms application option.
THe filewatcher has been created to watch files in a selected directory.
In the README.md, you will find a before report of the project and a after report of the project.

# Scenario
YOu are required to create a Windows Forms Application that can monitor a given directory on the computer’s hard drive. The user of the application should easily be able to browse to a specific folder on  the  computer  through  the  use  of  your  application  and  select  the  folder  to  monitor.  The  user should be able to choose when the application should start monitoring the chosen directory.All changes made in the specified directory (File creation, renaming, deleting and any other possible changes) needs to be recorded from your application, all of these changes need to be displayed on the form. As soon as a change is made in that directory, a notification should also be displayed at the bottom right corner of the screen of the computer that is running the application. The user should also have an option to include subdirectories of the directory they selected to also be monitored.

# Milestone 1

## Introduction
We are required to create a Windows Forms Application that can monitor a given directory on the
computer’s hard drive. The user of the application should easily be able to browse to a specific
folder on the computer through the use of your application and select the folder to monitor.
The user should be able to choose when the application should start monitoring the chosen
directory.

All changes made in the specified directory (File creation, renaming, deleting and any other possible
changes) needs to be recorded from your application, all of these changes need to be displayed on
the form. As soon as a change is made in that directory, a notification should also be displayed at the
bottom right corner of the screen of the computer that is running the application.
The user should also have an option to include subdirectories of the directory they selected to also
be monitored.
Flowchart 
Topics The project will be built using extensive research and knowledge in the following topics.
  • FileSystemWatcher
  • Windows Forms
  
**FileSystemWatcher Class**
The FileSystemWatcher class listens to the file system change notifications and will then raise events
when a directory, or file inside of a directory, changes.
There are some key important points to remember when using the FileSystemWatcher class.
-Hidden files are not ignored.
- In some systems, FileSystemWatcher reports changes to files using the short 8.3 file name
    format. For example, a change to "LongFileName.LongExtension" could be reported as
    "LongFil~.Lon".
- This class contains a link demand and an inheritance demand at the class level that applies
    to all members. A SecurityException is thrown when either the immediate caller or the
    derived class does not have full-trust permission. For details about security demands, see
    Link Demands.
- The maximum size you can set for the InternalBufferSize property for monitoring a directory
    over the network is 64 KB.
    
Use FileSystemWatcher to watch for changes in a specified directory. You can watch for changes in
files and subdirectories of the specified directory. You can create a component to watch files on a
local computer, a network drive, or a remote computer.
To watch for changes in all files, set the Filter property to an empty string ("") or use wildcards
("*.*"). To watch a specific file, set the Filter property to the file name. For example, to watch for
changes in the file MyDoc.txt, set the Filter property to "MyDoc.txt". You can also watch for changes
in a certain type of file. For example, to watch for changes in text files, set the Filter property to
"*.txt".

There are several types of changes you can watch for in a directory or file. For example, you can
watch for changes in Attributes, the LastWrite date and time, or the Size of files or directories. This is
done by setting the NotifyFilter property to one of the NotifyFilters values. For more information on
the type of changes you can watch, see NotifyFilters.
You can watch for renaming, deletion, or creation of files or directories. For example, to watch for
renaming of text files, set the Filter property to "*.txt" and call the WaitForChanged method with
a Renamed specified for its parameter.

The Windows operating system notifies your component of file changes in a buffer created by
the FileSystemWatcher. If there are many changes in a short time, the buffer can overflow. This
causes the component to lose track of changes in the directory, and it will only provide blanket
notification. Increasing the size of the buffer with the InternalBufferSize property is expensive, as it
comes from non-paged memory that cannot be swapped out to disk, so keep the buffer as small yet
large enough to not miss any file change events. To avoid a buffer overflow, use
the NotifyFilter and IncludeSubdirectories properties so you can filter out unwanted change
notifications.

**Design**
After designing the form, we took a screenshot of the front-end design, before implementing the
back-end coding to handle the events required.
The Application will only have one form where the directories and events are going to be displayed
with relevant buttons that handle the events.
In the above photo you will see that there are a total of 3 buttons, the idea is to have all 3 buttons
trigger different events.
![alt text](https://github.com/BroSnuffles/Filewatcher-WinForms/Assets/monitor.PNG)
- Monitor button
  - The monitor button will trigger the StartFileSystemWatcher() method, in the
      method we will watch the specified directory that is currently being viewed by the
      user inside of the Application.
- Stop Monitoring button
  - The stop monitoring button will trigger the StopFileSystemWatcher() method, in the
      method we will stop the FileSystemWatcher.
- Exit button
  - The exit button will serve the same purpose as the red x button at the top right
      corner of the application, it will allow the user to quit the application safely.
