import os
import re
import random
import requests
import urllib3
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

ROOT_FOLDER = r"E:\PDF\Programming"

API_URL = "https://localhost:7108/api/Files"

TOKENS = {
    "admin1": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIxIiwiZXhwIjoxNzczNTMwMzA5LCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.B_lDfKYVZt64u7JkkJr-UWDjrfGlna7A4SX5d7aeK1g",

    "admin2": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMiIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIyIiwiZXhwIjoxNzczNTMyMTg4LCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.RPU4CASZd3ke6goJjG7iaCs89hGdkVKS0LKjV85aI3s",

    "admin3": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluMyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiTGV2ZWwiOiIzIiwiZXhwIjoxNzczNTMyMjAzLCJpc3MiOiJLaG9hQ05UVF9BUEkiLCJhdWQiOiJLaG9hQ05UVF9Vc2VyIn0.foOm7-iP-Fda2FXAfBwnrR5Un3_joKHquN50mrs3yT8"
}

SUBJECT_CODES = [
    "CSE105",
    "CSE111",
    "CSE112",
    "CSE106",
    "CSE115",
    ""
]

PERMISSIONS = [
    "Hidden",
    "PublicRead",
    "PublicDownload",
    "StudentRead",
    "StudentDownload"
]

FILE_TYPES = [
    "LectureNotes",
    "Test",
    "Form",
    "Other"
]


def generate_title(filename):
    name = filename.rsplit(".", 1)[0]

    name = name.replace("_", " ")
    name = name.replace("-", " ")

    name = re.sub(r"\d+", "", name)
    name = re.sub(r"\s+", " ", name)

    name = name.strip()

    if len(name) > 60:
        name = name[:60]

    return name.title()


def collect_files(root):
    all_files = []

    for path, dirs, files in os.walk(root):
        for f in files:
            full = os.path.join(path, f)
            all_files.append(full)

    return all_files


def upload_file(filepath):

    filename = os.path.basename(filepath)

    title = generate_title(filename)

    subject = random.choice(SUBJECT_CODES)
    permission = random.choice(PERMISSIONS)
    filetype = random.choice(FILE_TYPES)

    role = random.choice(list(TOKENS.keys()))
    token = TOKENS[role]

    headers = {
        "Authorization": f"Bearer {token}"
    }

    files = {
        "File": (filename, open(filepath, "rb"))
    }

    data = {
        "Title": title,
        "SubjectCode": subject,
        "Permission": permission,
        "FileType": filetype
    }

    try:
        res = requests.post(API_URL, headers=headers, data=data, files=files, verify=False)

        print(f"{role} | {filename} | {res.status_code}")

    except Exception as e:
        print("Error:", filepath, e)


def main():

    files = collect_files(ROOT_FOLDER)

    random.shuffle(files)

    for f in files:
        upload_file(f)


if __name__ == "__main__":
    main()