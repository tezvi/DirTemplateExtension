<div id="top"></div>

<!-- PROJECT SHIELDS -->
[![MIT License][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]


<!-- PROJECT LOGO -->
<br />
<div align="center">

<h3 align="center">DirTemplateExtension - Windows shell extension for bootstrapping projects</h3>

  <p align="center">
    Use Windows explorer context menu to initialize new directories from project templates
</div>



<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
      <ul>
        <li><a href="#built-with">Built With</a></li>
      </ul>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>



<!-- ABOUT THE PROJECT -->
## About The Project

DirTemplateExtension is a COM server shell extension that integrates into Windows Explorer right-click context menu for directories and directory backgrounds. It allows you to boostrap new project directories from predefined templates. Template directories are defined in user's My Documents subdirectory `DirTemplates` as a first level subdirectories. Each template can contain any number of files and subdirectories regardless of depth. 

You may place a file named `project.png` in template directory and extension will load it under Windows Explorer context menu item. 

![DirTemplateExtension Screen Shot][product-screenshot]

Shell extension will automatically pick up any changes in template directory names and render context submenus accordingly.
Extension supports multiple directory selection.

![DirTemplateExtension intro dialog][product-screenshot2]

### Debugging

Extension logs standard information, warning and error logs to Windows System Event log under Application logs for event source of DirTemplateExtension.

#### Advanced debug logging
Create an empty file `.debug` in template directory `{User}\MyDocuments\DirTemplates`. This file will signal extension to start outputting debug logs to a local file `{User}\MyDocuments\DirTemplates\debug.log`. When finished debugging you may delete `.debug` file and extension will stop logging.

<p align="right">(<a href="#top">back to top</a>)</p>



### Built With

* [C#](https://docs.microsoft.com/en-us/dotnet/csharp/)
* [SharpShell](https://github.com/dwmkerr/sharpshell)
* [InnoSetup](https://jrsoftware.org)


<!-- GETTING STARTED -->
## Getting Started

Checkout this project and open it with C# supported IDE.
You may use VSCode or VisualStudio Community.

### Prerequisites

Make sure you have .NET SDK 4.6.x or similar that is supported by the SharpShell project.
Download SharpShell tools and libraries from [SharpShell](https://github.com/dwmkerr/sharpshell) and place them in SharpShellTools subdirectory. You may want to use Nuget packages for initializing SharpShell in your project.
SharpShell tools are needed for building an installer and COM debugging but are not mandatory for a DLL build. SharpShell libraries are mandatory.
To create installer you need to install [InnoSetup](https://jrsoftware.org).

### Build and install

1. Checkout source as a new VisualStudio project.
2. Clone the repo
   ```sh
   git clone https://github.com/tezvi/DirTemplateExtension.git
   ```
3. Download SharpShell library and SharpShell tools.
4. Build VisualStudio project for CPU architecture that is supported by your SharpShell library version.
5. Compile installer by using InnoSetup IDE.
6. Install compiled EXE installer on target computer.

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- CONTACT -->
## Contact

Project Link: [https://github.com/tezvi/DirTemplateExtension](https://github.com/tezvi/DirTemplateExtension)

<p align="right">(<a href="#top">back to top</a>)</p>


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[license-shield]: https://img.shields.io/github/license/tezvi/DirTemplateExtension.svg?style=for-the-badge
[license-url]: https://github.com/tezvi/DirTemplateExtension/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/andrej-v-11481925/
[product-screenshot]: docs/context_menu.png
[product-screenshot2]: docs/template_dir.png
