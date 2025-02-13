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
    - load_user_data(): Sends a GET request to load user data by ID or EduCard.
    - update_ui(): Updates the UI based on the selected operation.
    - clear_all_inputs(): Clears all input fields in the GUI.

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
    - clear_button: Button to clear all input fields.

Usage:
    Run the script to launch the GUI. Select an operation (create, delete, update)
    and fill in the necessary fields. Click "Absenden" to send the data to the API.
    Click "Clear All" to reset all input fields.
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
        "educard": edu_card_entry.get() if edu_card_value else None,
    }

    headers = {'Content-Type': 'application/json'}

    if class_value and edu_card_value:
        url = 'https://192.168.68.116:44320/create-user-that-has-everything'
    elif class_value:
        url = 'https://192.168.68.116:44320/create-user-that-has-no-educard'
    else:
        url = 'https://192.168.68.116:44320/create-user-that-has-no-educard-and-no-class'

    try:
        response = requests.post(url, json=data, headers=headers, verify=False)

        # Check if response has content
        if response.status_code == 200 and response.text:
            response_data = response.json()  # Safe to parse JSON now
            user_id = response_data.get('id')
            id_label.config(text=f"Letzte erstellte ID: {user_id}")
            messagebox.showinfo("Erfolg", f"Daten erfolgreich gesendet!\nID: {user_id}")
        else:
            messagebox.showerror("Fehler", f"Fehler: {response.status_code}\nKeine Daten zurückgegeben.")

    except requests.exceptions.RequestException as e:
        messagebox.showerror("Fehler", f"Netzwerkfehler: {str(e)}")
    except ValueError:
        messagebox.showerror("Fehler", "Ungültige JSON-Antwort vom Server.")


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
    Sends a GET request to load user data by EduCard or ID.
    """
    edu_card_number = edu_card_entry.get().strip()
    id_number = id_entry.get().strip()

    # Ensure at least one input is provided
    if not edu_card_number and not id_number:
        messagebox.showerror("Fehler", "Bitte geben Sie eine ID oder eine EduCard-Nummer ein.")
        return

    # Ensure only one input is provided
    if edu_card_number and id_number:
        messagebox.showerror("Fehler", "Du kannst nur nach einem Parameter suchen (ID oder EduCard).")
        return

    url = ''
    if edu_card_number:
        try:
            edu_card_number = float(edu_card_number)  # Ensure valid number
            url = f'https://192.168.68.116:44320/api/ReadUserEdu?educardNumber={edu_card_number}'
        except ValueError:
            messagebox.showerror("Fehler", "Ungültige EduCard-Nummer.")
            return
    elif id_number:
        try:
            id_number = int(id_number)  # Ensure valid integer
            url = f'https://192.168.68.116:44320/api/ReadUserID?id={id_number}'
        except ValueError:
            messagebox.showerror("Fehler", "Ungültige Benutzer-ID.")
            return

    headers = {'Content-Type': 'application/json'}

    try:
        response = requests.get(url, headers=headers, verify=False)

        if response.status_code == 200:
            user_data = response.json()

            if not user_data or user_data == {}:
                messagebox.showwarning("Warnung", "Keine Daten gefunden.")
                return

            # Helper function to replace {} with an empty string
            def clean_value(value):
                return "" if value == {} else value

            # Populate fields
            id_entry.delete(0, tk.END)
            id_entry.insert(0, str(clean_value(user_data.get('id', ''))))  # ID field updated

            firstname_entry.delete(0, tk.END)
            firstname_entry.insert(0, clean_value(user_data.get('firstName', '')))

            lastname_entry.delete(0, tk.END)
            lastname_entry.insert(0, clean_value(user_data.get('lastName', '')))

            org_entry.delete(0, tk.END)
            org_entry.insert(0, clean_value(user_data.get('organisation', '')))

            class_entry.delete(0, tk.END)
            class_entry.insert(0, clean_value(user_data.get('schoolClass', '')))

            edu_card_entry.delete(0, tk.END)
            edu_card_entry.insert(0, str(clean_value(user_data.get('eduCardNumber', ''))))

            messagebox.showinfo("Erfolg", "Daten erfolgreich geladen!")
        elif response.status_code == 400:
            messagebox.showerror("Fehler", "Ungültige Anfrage. Bitte überprüfen Sie die Eingabe.")
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


def clear_all_inputs():
    """
    Clears all input fields in the GUI.
    """
    id_entry.delete(0, tk.END)
    firstname_entry.delete(0, tk.END)
    lastname_entry.delete(0, tk.END)
    org_entry.delete(0, tk.END)
    class_entry.delete(0, tk.END)
    edu_card_entry.delete(0, tk.END)
    id_label.config(text="Letzte erstellte ID: Keine")


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
submit_button.grid(row=10, column=1, padx=5, pady=10)

# Load button to load user data
load_button = ttk.Button(root, text="Laden", command=load_user_data)
load_button.grid(row=10, column=2, padx=5, pady=10)
load_button.grid_remove()  # Initially hidden

# Clear All button to reset all input fields
clear_button = ttk.Button(root, text="Clear All", command=clear_all_inputs)
clear_button.grid(row=10, column=0, padx=5, pady=10)


# Initialize the UI based on the selected operation
update_ui()

# Start the main event loop
root.mainloop()