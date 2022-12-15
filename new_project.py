import urllib.request
import os
import shutil
import re
import uuid

year = 2022
sln_file = f"Advent_of_Code_{year}.sln"
template_project = "TemplateProject"

def new_project(new_project_name):    
    try:
        with open(sln_file, 'r') as file:
            sln_contents = file.read()
    except FileNotFoundError:
        print ("Solution file `{sln_file}` not found")
    else:
        regex = r"(\s*Project\s*?\(\"{[^}]+}\"\)\s*?=\s*?\")" + template_project + "(\"\s*?,\s*?\")[^\"]+(\"\s*?,\s*?\"{)[^}]+(}\"\s*?EndProject)"
        m = re.search(regex, sln_contents)
        if m:
            print(f"Generating new project definition `{new_project_name}` for `{sln_file}`")
            guid = str(uuid.uuid4()).upper()
            new_proj_def = f"{m.group(1)}{new_project_name}{m.group(2)}{os.path.join(new_project_name, new_project_name + '.csproj')}{m.group(3)}{guid}{m.group(4)}"
            
            print(f"Making directory `{new_project_name}`")
            os.mkdir(new_project_name)
            
            src_input = os.path.join(template_project, "input.txt")
            dst_input = os.path.join(new_project_name, "input.txt")
            print(f"Copying `{src_input}` to `{dst_input}`")
            shutil.copyfile(src_input, dst_input)
            src_proj = os.path.join(template_project, "TemplateProject.csproj")
            dst_proj = os.path.join(new_project_name, f"{new_project_name}.csproj")
            print(f"Copying `{src_proj}` to `{dst_proj}`")
            shutil.copyfile(src_proj, dst_proj)
            src_cs = os.path.join(template_project, "Program.cs")
            dst_cs = os.path.join(new_project_name, "Program.cs")
            print(f"Copying `{src_cs}` to `{dst_cs}`")
            shutil.copyfile(src_cs, dst_cs)
            
            print(f"Adding new project `{template_project}` to `{sln_file}`")
            replace_regex = r"(.*Project\s*?\(\"{[^}]+}\"\)\s*?=\s*?\"[^\"]+\"\s*?,\s*?\"[^\"]+\"\s*?,\s*?\"{[^}]+}\"\s*?EndProject)"
            new_sln_contents = re.sub(replace_regex, "\\1" + new_proj_def.replace("\\","\\\\"), sln_contents, 1, re.DOTALL)
            
            with open(sln_file, 'w') as file:
                sln_contents = file.write(new_sln_contents)
        else:
            print(f"Project with title `{template_project}` not found in `{sln_file}`")

def main():
    while True:
        day = int(input("Day: "))
        url = f"https://adventofcode.com/{year}/day/{day}"
        try:
            http_response = urllib.request.urlopen(url)
        except urllib.error.HTTPError:
            continue
        else:
            break
    html = str(http_response.read())
    open_del = "<h2>--- "
    close_del = " ---</h2>"
    open_h2 = html.index(open_del)
    close_h2 = html.index(close_del)
    title = html[open_h2 + len(open_del):close_h2]
    print(f"Original title is `{title}`")
    renamed_title = title.replace(":", "").replace(" ", "_")
    print(f"Suggested project title is `{renamed_title}`")
    r = ""
    while r != "y" and r != "n":
        r = input("Accept suggested title? [Y/n] ").lower()
        if r == "":
            r = "y"
    if r == "n":
        r = ""
        while r == "":
            r = input(f"What should the new project be called? Day_{day}_")
        r = f"Day_{day}_{r}"
    else:
        r = renamed_title
    print(f"Using project name `{r}`")
    print(f"Checking if directory `{r}` already exists")
    if (os.path.exists(r)):
        print(f"Directory `{r}` already exists")
    else:
        new_project(r)

main()
