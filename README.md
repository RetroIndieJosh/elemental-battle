This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <https://www.gnu.org/licenses/>.

Encounters are really enemy GROUPS

An EncounterRegion has a collection of potential Encounters (not to be confused with the class named Encounter)
        Each has an arbitrary number of Encounter GROUPS collected together

We roll one from the list and add ALL enemies in ALL Encounters added
