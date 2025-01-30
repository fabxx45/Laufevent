"""
API Data Input Form

This script provides a graphical user interface (GUI) for interacting with a REST API. 
The GUI allows users to create, delete, and update user records by sending HTTP requests 
to the API. The interface is built using the `tkinter` library.

Modules:
    - tkinter: Used for creating the GUI.
    - requests: Used for sending HTTP requests to the API.
    - messagebox: Used for displaying messages to the user.
    - ttk: Themed tkinter widgets for a better look and feel.

Functions:
    - send_to_api(): Determines which operation to perform based on user selection.
    - create_user(): Sends a POST request to create a new user.
    - delete_user_by_id(): Sends a DELETE request to delete a user by ID.
    - update_user_by_id(): Sends a PUT request to update a user by ID.
    - load_user_data(): Sends a GET request to load user data by ID.
    - update_ui(): Updates the UI based on the selected operation.

Classes:
    - None

Variables:
    - root: The main window of the application.
    - operation_var: Tracks the selected operation (create, delete, update).
    - id_label: Displays the last created user ID.
    - id_entry: Entry field for user ID.
    - firstname_entry, lastname_entry, org_entry, class_entry, edu_card_entry: 
      Entry fields for user details.
    - load_button: Button to load user data.
    - submit_button: Button to submit the form.

Usage:
    Run the script to launch the GUI. Select an operation (create, delete, update) 
    and fill in the necessary fields. Click "Absenden" to send the data to the API.
"""

import tkinter as tk
from tkinter import messagebox
from tkinter import ttk
import requests

def send_to_api():
    """
    Determines which operation to perform based on the selected radio button.

    Calls the appropriate function (create_user, delete_user_by_id, update_user_by_id)
    based on the value of operation_var.
    """
    if operation_var.get() == 1:  # Create User
        create_user()
    elif operation_var.get() == 2:  # Delete User
        delete_user_by_id()
    elif operation_var.get() == 3:  # Update User
        update_user_by_id()

def create_user():
    """
    Sends a POST request to create a new user.

    Collects data from the entry fields, constructs a JSON payload, and sends it 
    to the appropriate API endpoint based on the presence of optional fields.
    """
    class_value = class_entry.get()
    edu_card_value = edu_card_entry.get()

    data = {
        "firstname": firstname_entry.get(),
        "lastname": lastname_entry.get(),
        "organisation": org_entry.get(),
        "school_class": class_entry.get() if class_value else None,
        "educard": edu_card_entry.get() if edu_card_value else None
    }

    headers = {'Content-Type': 'application/json'}

    if class_value and edu_card_value:
        url = 'https://192.168.68.116:44320/create user that has everything'
    elif edu_card_value:
        url = 'https://192.168.68.116:44320/create user that has no educard and no class'
    elif class_value:
        url = 'https://192.168.68.116:44320/create user that has no educard'
    else:
        url = 'https://192.168.68.116:44320/create user that has no educard and no class'

    try:
        response = requests.post(url, json=data, headers=headers, verify=False)
        response_data = response.json()
        if response.status_code == 200:
            user_id = response_data.get('id')
            id_label.config(text=f"Letzte erstellte ID: {user_id}")
            messagebox.showinfo("Erfolg", f"Daten erfolgreich gesendet!\nID: {user_id}")
        else:
            messagebox.showerror("Fehler", f"Fehler: {response.status_code}\n{response.text}")
    except Exception as e:
        messagebox.showerror("Fehler", str(e))

def delete_user_by_id():
    """
    Sends a DELETE request to delete a user by ID.

    Collects the user ID from the entry field and sends a DELETE request to the API.
    """
    user_id = id_entry.get()
    if not user_id:
        messagebox.showwarning("Warnung", "Bitte geben Sie eine ID ein.")
        return

    url = f'https://192.168.68.116:44320/api/DeleteUserById/{user_id}'
    headers = {'Content-Type': 'application/json'}

    try:
        response = requests.delete(url, headers=headers, verify=False)
        if response.status_code == 200:
            messagebox.showinfo("Erfolg", "Benutzer erfolgreich gelöscht!")
            id_label.config(text="Letzte erstellte ID: Keine")
        else:
            messagebox.showerror("Fehler", f"Fehler: {response.status_code}\n{response.text}")
    except Exception as e:
        messagebox.showerror("Fehler", str(e))

def update_user_by_id():
    """
    Sends a PUT request to update a user by ID.

    Collects the user ID and updated data from the entry fields, constructs a JSON payload, 
    and sends it to the API.
    """
    user_id = id_entry.get()
    if not user_id:
        messagebox.showwarning("Warnung", "Bitte geben Sie eine ID ein.")
        return

    data = {
        "firstName": firstname_entry.get() or None,
        "lastName": lastname_entry.get() or None,
        "eduCardNumber": edu_card_entry.get() or None,
        "schoolClass": class_entry.get() or None,
        "organisation": org_entry.get() or None
    }

    url = f'https://192.168.68.116:44320/api/UpdateUser?id={user_id}'
    headers = {'Content-Type': 'application/json'}

    try:
        response = requests.put(url, json=data, headers=headers, verify=False)
        if response.status_code == 200:
            messagebox.showinfo("Erfolg", "Benutzer erfolgreich aktualisiert!")
        else:
            messagebox.showerror("Fehler", f"Fehler: {response.status_code}\n{response.text}")
    except Exception as e:
        messagebox.showerror("Fehler", str(e))

def load_user_data():
    """
    Sends a GET request to load user data by ID.

    Collects the user ID from the entry field, sends a GET request to the API, 
    and populates the entry fields with the retrieved data.
    """
    user_id = id_entry.get()
    if not user_id:
        messagebox.showwarning("Warnung", "Bitte geben Sie eine ID ein.")
        return

    url = f'https://192.168.68.116:44320/api/ReadUserID?id={user_id}'
    headers = {'Content-Type': 'application/json'}

    try:
        response = requests.get(url, headers=headers, verify=False)
        if response.status_code == 200:
            user_data = response.json()

            firstname_value = user_data.get('firstName')
            lastname_value = user_data.get('lastName')
            org_value = user_data.get('organisation')
            class_value = user_data.get('schoolClass')
            edu_card_value = user_data.get('eduCardNumber')

            firstname_entry.delete(0, tk.END)
            firstname_entry.insert(0, '' if firstname_value in [None, {}] else firstname_value)

            lastname_entry.delete(0, tk.END)
            lastname_entry.insert(0, '' if lastname_value in [None, {}] else lastname_value)

            org_entry.delete(0, tk.END)
            org_entry.insert(0, '' if org_value in [None, {}] else org_value)

            class_entry.delete(0, tk.END)
            class_entry.insert(0, '' if class_value in [None, {}] else class_value)

            edu_card_entry.delete(0, tk.END)
            edu_card_entry.insert(0, '' if edu_card_value in [None, {}] else edu_card_value)

            messagebox.showinfo("Erfolg", "Daten erfolgreich geladen!")
        else:
            messagebox.showerror("Fehler", f"Fehler: {response.status_code}\n{response.text}")
    except Exception as e:
        messagebox.showerror("Fehler", str(e))

def update_ui():
    """
    Updates the UI based on the selected operation.

    Shows or hides entry fields and buttons depending on whether the user is creating, 
    deleting, or updating a user.
    """
    if operation_var.get() == 1:  # Create User
        id_label_entry.grid_remove()
        id_entry.grid_remove()
        firstname_label.grid()
        firstname_entry.grid()
        lastname_label.grid()
        lastname_entry.grid()
        org_label.grid()
        org_entry.grid()
        class_label.grid()
        class_entry.grid()
        edu_card_label.grid()
        edu_card_entry.grid()
        load_button.grid_remove()  # Hide the Load button
    elif operation_var.get() == 2:  # Delete User
        id_label_entry.grid()
        id_entry.grid()
        firstname_label.grid_remove()
        firstname_entry.grid_remove()
        lastname_label.grid_remove()
        lastname_entry.grid_remove()
        org_label.grid_remove()
        org_entry.grid_remove()
        class_label.grid_remove()
        class_entry.grid_remove()
        edu_card_label.grid_remove()
        edu_card_entry.grid_remove()
        load_button.grid_remove()  # Hide the Load button
    elif operation_var.get() == 3:  # Update User
        id_label_entry.grid()
        id_entry.grid()
        firstname_label.grid()
        firstname_entry.grid()
        lastname_label.grid()
        lastname_entry.grid()
        org_label.grid()
        org_entry.grid()
        class_label.grid()
        class_entry.grid()
        edu_card_label.grid()
        edu_card_entry.grid()
        load_button.grid()  # Show the Load button

# Main application window
root = tk.Tk()
root.title("API-Daten Eingabeformular")

# Variable to track the selected operation
operation_var = tk.IntVar(value=1)

# Label to display the last created user ID
id_label = tk.Label(root, text="Letzte erstellte ID: Keine", font=("Helvetica", 12), fg="blue")
id_label.grid(row=0, column=0, columnspan=3, pady=5)

# Entry fields and labels for user details
id_label_entry = tk.Label(root, text="User ID:")
id_label_entry.grid(row=1, column=0, padx=10, pady=5)

id_entry = tk.Entry(root)
id_entry.grid(row=1, column=1, padx=10, pady=5)

firstname_label = tk.Label(root, text="Firstname:")
firstname_label.grid(row=2, column=0, padx=10, pady=5)

firstname_entry = tk.Entry(root)
firstname_entry.grid(row=2, column=1, padx=10, pady=5)

lastname_label = tk.Label(root, text="Lastname:")
lastname_label.grid(row=3, column=0, padx=10, pady=5)

lastname_entry = tk.Entry(root)
lastname_entry.grid(row=3, column=1, padx=10, pady=5)

org_label = tk.Label(root, text="Organisation:")
org_label.grid(row=4, column=0, padx=10, pady=5)

org_entry = tk.Entry(root)
org_entry.grid(row=4, column=1, padx=10, pady=5)

class_label = tk.Label(root, text="Class:")
class_label.grid(row=5, column=0, padx=10, pady=5)

class_entry = tk.Entry(root)
class_entry.grid(row=5, column=1, padx=10, pady=5)

edu_card_label = tk.Label(root, text="Edu Card:")
edu_card_label.grid(row=6, column=0, padx=10, pady=5)

edu_card_entry = tk.Entry(root)
edu_card_entry.grid(row=6, column=1, padx=10, pady=5)

# Radio buttons for selecting the operation
style = ttk.Style()
style.configure('My.TButton', font=('Helvetica', 10))

ttk.Radiobutton(root, text="Benutzer erstellen", variable=operation_var, value=1, style='My.TButton', command=update_ui).grid(row=9, column=0, padx=5, pady=5)
ttk.Radiobutton(root, text="Benutzer löschen", variable=operation_var, value=2, style='My.TButton', command=update_ui).grid(row=9, column=1, padx=5, pady=5)
ttk.Radiobutton(root, text="Benutzer aktualisieren", variable=operation_var, value=3, style='My.TButton', command=update_ui).grid(row=9, column=2, padx=5, pady=5)

# Submit button to send data to the API
submit_button = ttk.Button(root, text="Absenden", command=send_to_api)
submit_button.grid(row=10, column=0, columnspan=3, pady=10)

# Load button to load user data
load_button = ttk.Button(root, text="Laden", command=load_user_data)
load_button.grid(row=10, column=0, columnspan=2, pady=10)
load_button.grid_remove()

# Initialize the UI based on the selected operation
update_ui()

# Start the main event loop
root.mainloop()