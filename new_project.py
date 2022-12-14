import urllib.request
import os
import shutil

year = 2022
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
    print(f"Making directory `{r}`")
    os.mkdir(r)
    template_directory = "TemplateProject"
    
    src_input = os.path.join(template_directory, "input.txt")
    dst_input = os.path.join(r, "input.txt")
    print(f"Copying `{src_input}` to `{dst_input}`")
    shutil.copyfile(src_input, dst_input)
    src_proj = os.path.join(template_directory, "TemplateProject.csproj")
    dst_proj = os.path.join(r, f"{r}.csproj")
    print(f"Copying `{src_proj}` to `{dst_proj}`")
    shutil.copyfile(src_proj, dst_proj)
    src_cs = os.path.join(template_directory, "Program.cs")
    dst_cs = os.path.join(r, "Program.cs")
    print(f"Copying `{src_cs}` to `{dst_cs}`")
    shutil.copyfile(src_cs, dst_cs)
