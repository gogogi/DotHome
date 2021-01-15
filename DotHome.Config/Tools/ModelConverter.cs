using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DotHome.Config.Tools
{
    public static class ModelConverter
    {
        //-------------------------------------------------------- View To Model -------------------------------------------------------------------------
        
        public static Project ProjectViewToProject(ProjectView projectView)
        {
            Project project = new Project();
            foreach(PageView pageView in projectView.Pages)
            {
                project.Pages.Add(PageViewToPage(pageView));
            }
            return project;
        }
        
        public static Page PageViewToPage(PageView pageView)
        {
            Page page = new Page() { Name = pageView.Name, Width = pageView.Width, Height = pageView.Height };

            Dictionary<InputView, IInput> inputsDictionary = new Dictionary<InputView, IInput>();
            Dictionary<OutputView, IOutput> outputsDictionary = new Dictionary<OutputView, IOutput>();

            foreach(ABlockView aBlockView in pageView.Blocks)
            {
                page.Blocks.Add(ABlockViewToABlock(aBlockView, inputsDictionary, outputsDictionary));
            }

            foreach(WireView wireView in pageView.Wires)
            {
                page.Wires.Add(new Wire() { Input = inputsDictionary[wireView.InputView], Output = outputsDictionary[wireView.OutputView] });
            }

            return page;
        }

        public static ABlock ABlockViewToABlock(ABlockView aBlockView, Dictionary<InputView, IInput> inputsDictionary, Dictionary<OutputView, IOutput> outputsDictionary)
        {
            if (aBlockView is RefSinkView rsiv) return RefSinkViewToRefSink(rsiv, inputsDictionary, outputsDictionary);
            else if (aBlockView is RefSourceView rsov) return RefSourceViewToRefSource(rsov, inputsDictionary, outputsDictionary);
            else if (aBlockView is ConstView cv) return ConstViewToConst(cv, inputsDictionary, outputsDictionary);

            return null;
        }

        public static Const ConstViewToConst(ConstView cv, Dictionary<InputView, IInput> inputsDictionary, Dictionary<OutputView, IOutput> outputsDictionary)
        {
            var c = new Const() { Id = cv.Id, Type = cv.Type, Value = cv.Value, X = (int)cv.X, Y = (int)cv.Y };
            outputsDictionary.Add(cv.Outputs[0], c);
            return c;
        }

        public static RefSink RefSinkViewToRefSink(RefSinkView refSinkView, Dictionary<InputView, IInput> inputsDictionary, Dictionary<OutputView, IOutput> outputsDictionary)
        {
            var refSink = new RefSink() { Id = refSinkView.Id, Reference = refSinkView.Reference, X = (int)refSinkView.X, Y = (int)refSinkView.Y };
            inputsDictionary.Add(refSinkView.Inputs[0], refSink);
            return refSink;
        }

        public static RefSource RefSourceViewToRefSource(RefSourceView refSourceView, Dictionary<InputView, IInput> inputsDictionary, Dictionary<OutputView, IOutput> outputsDictionary)
        {
            var refSource = new RefSource() { Id = refSourceView.Id, Reference = refSourceView.Reference, X = (int)refSourceView.X, Y = (int)refSourceView.Y };
            outputsDictionary.Add(refSourceView.Outputs[0], refSource);
            return refSource;
        }

        //-------------------------------------------------------- Model To View -------------------------------------------------------------------------

        public static ProjectView ProjectToProjectView(Project project)
        {
            ProjectView projectView = new ProjectView();
            foreach (Page page in project.Pages)
            {
                projectView.AddPage(PageToPageView(page));
            }
            return projectView;
        }

        public static PageView PageToPageView(Page page)
        {
            PageView pageView = new PageView() { Name = page.Name, Width = page.Width, Height = page.Height };

            Dictionary<IInput, InputView> inputsDictionary = new Dictionary<IInput, InputView>();
            Dictionary<IOutput, OutputView> outputsDictionary = new Dictionary<IOutput, OutputView>();

            foreach (ABlock aBlock in page.Blocks)
            {
                pageView.AddBlock(ABlockToABlockView(aBlock, inputsDictionary, outputsDictionary));
            }

            foreach (Wire wire in page.Wires)
            {
                pageView.AddWire(new WireView(inputsDictionary[wire.Input], outputsDictionary[wire.Output]));
            }

            return pageView;
        }

        public static ABlockView ABlockToABlockView(ABlock aBlock, Dictionary<IInput, InputView> inputsDictionary, Dictionary<IOutput, OutputView> outputsDictionary)
        {
            if (aBlock is RefSink rsi) return RefSinkToRefSinkView(rsi, inputsDictionary, outputsDictionary);
            else if (aBlock is RefSource rso) return RefSourceToRefSourceView(rso, inputsDictionary, outputsDictionary);
            else if (aBlock is Const c) return ConstToConstView(c, inputsDictionary, outputsDictionary);
            return null;
        }

        public static ConstView ConstToConstView(Const c, Dictionary<IInput, InputView> inputsDictionary, Dictionary<IOutput, OutputView> outputsDictionary)
        {
            var constView = new ConstView() { Id = c.Id, Type = c.Type, Value = c.Value, X = c.X, Y = c.Y };
            outputsDictionary.Add(c, constView.Outputs[0]);
            return constView;
        }

        public static RefSourceView RefSourceToRefSourceView(RefSource refSource, Dictionary<IInput, InputView> inputsDictionary, Dictionary<IOutput, OutputView> outputsDictionary)
        {
            var refSourceView = new RefSourceView() { Id = refSource.Id, X = refSource.X, Y = refSource.Y, Reference = refSource.Reference };
            outputsDictionary.Add(refSource, refSourceView.Outputs[0]);
            return refSourceView;
        }

        public static RefSinkView RefSinkToRefSinkView(RefSink refSink, Dictionary<IInput, InputView> inputsDictionary, Dictionary<IOutput, OutputView> outputsDictionary)
        {
            var refSinkView = new RefSinkView() { Id = refSink.Id, X = refSink.X, Y = refSink.Y, Reference = refSink.Reference };
            inputsDictionary.Add(refSink, refSinkView.Inputs[0]);
            return refSinkView;
        }
    }
}
