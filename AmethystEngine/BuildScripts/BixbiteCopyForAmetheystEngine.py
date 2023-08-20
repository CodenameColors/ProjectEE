import os
import sys
import shutil
import glob
from os.path import join, isfile
from shutil import copytree


def copy_files_recrusively(src, dest):
    # ignore any files but files with '.h' extension
    ignore_func = lambda d, files: [f for f in files if isfile(join(d, f)) and f[-2:] == '.cs']
    copytree(src, dest, ignore=ignore_func)

print('Starting Bixbite Copy Build Script --  {}'.format((__file__)))

# we need to get all the files from Bixbite, and copy them to NodeEditor/Bixbite_windows
amethyst_engine_inner_path = "AmethystEngine\\Components\\Bixbite_Windows_Build"
output_path = '{}{}'.format(os.getcwd(), "\\..\\..\\..\\")

if (output_path.find("ProjectEE") == -1):
    print('Exiting script not needed for non amethyst engine projects--  {} '.format((__file__)))
    
output_path += amethyst_engine_inner_path

# step 1. Delete the desired folder.
if os.path.exists(output_path) and os.path.isdir(output_path):
    # print("Folder exists")
    shutil.rmtree(output_path)

# step 2. Create the folders again nice and clean
# os.makedirs(output_path)
# os.makedirs('{}{}'.format(output_path, "\\Resources"))

# step 3. Copy all the bixbite files.
copy_files_recrusively('{}{}'.format(os.getcwd(), "\\..\\..\\..\\Bixbite\\"), output_path)

print('End of Bixbite Copy Build Script --  {}'.format((__file__)))